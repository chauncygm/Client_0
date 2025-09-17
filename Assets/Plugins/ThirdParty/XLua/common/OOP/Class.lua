--[[
-- Lua面向对象设计
--]]

---用于IDE定位
---@class Class

---@class ClassType 自定义类型
local ClassType = {
    CLASS = 1,
    INSTANCE = 2
}
_G.ClassType = ClassType

---拷贝字段时不进行拷贝的字段
local _ignoreKeys = {
    New = true,
    __new = true,
    __dispose = true,
    Dispose = true,

    -- cls成员
    super = true,
    __cname = true,
    __ctype = true,
    __staticMembers = true,
    __class = true,

    -- lua的一些元方法，元方法尽量都不拷贝
    __index = true,
    __newindex = true,
    __call = true,
}

--已声明的类，用于避免重复定义
local class_dic = {}
local rawget = rawget

--- 所有Lua类的基类
---注意：
---不能直接通过self.super或self.__class.super来调用超类方法
---因为self始终是当前对象，进入父类方法后，如果再次调用self.super会产生死循环
---@class LuaObject
---@field super LuaObject|self @模拟父类
---@field __cname string @类名
---@field __ctype ClassType @类的类型
---@field __class LuaObject 实例关联的class（类型自己也指向了自己）
---@field __staticMembers table 类的静态成员（字段和函数 -- 延迟初始化） 最好不要直接访问
local LuaObject = {}
LuaObject.__cname = "LuaObject"
LuaObject.__ctype = ClassType.CLASS
LuaObject.__class = LuaObject
LuaObject.__staticMembers = nil

---构造函数入口，不要重写该方法
---如果重写该方法，可手动调用 {@code NewInstance }方法
function LuaObject.New(...)

end

---构造函数
---会强制先调用父类__new方法，然后再调用当前类的__new方法，因此在重写该函数时不必手动调用super的__new方法
---参数较多时建议是以table，kv形式传值，'...'形式难以扩展和维护
function LuaObject:__new(...)

end

---析构函数
---会强制调用当前类的析构函数，然后再调用父类的析构函数，因此在重写该函数时不必手动调用super的__dispose方法。
function LuaObject:__dispose()

end

---析构函数入口，不要重写该方法
---如果重写该方法，可手动调用{@code DestroyInstance}方法
function LuaObject:Dispose()

end

---初始化对象的内存
---其实就是初始化内存的过程 （初始化变量的过程）
---create模拟面向对象里构造函数的递归调用(从父类开始构造)
---@param cls LuaObject
---@param instance LuaObject
local function initMemory(cls, instance, ...)
    -- 先递归到root super不在元表里，可以不rawget
    if cls.super then
        initMemory(cls.super, instance, ...)
    end

    -- 不可以访问元表，否则可能获取到父类的构造函数
    local constructor = rawget(cls, "__new")
    if constructor ~= nil then
        constructor(instance, ...)
    end
end

---创建一个class的实例，也可以用于创建抽象类和接口的实例
---@param cls LuaObject lua类型
---@return LuaObject
function NewInstance(cls, ...)
    ---@type LuaObject
    local instance = setmetatable({}, cls)
    instance.__class = cls
    instance.__ctype = ClassType.INSTANCE

    -- 分配内存
    initMemory(cls, instance, ...)

    return instance
end

---该方法是为了兼容老代码
---注意：该方法默认还是会走cls的构造方法的
---@param t table 一个已经初始化的table
---@param cls LuaObject 要转换的class类型
function ToInstanceOf(t, cls, ...)
    ---@type LuaObject
    local instance = setmetatable(t, cls)
    instance.__class = cls
    instance.__ctype = ClassType.INSTANCE

    -- 分配内存
    initMemory(cls, instance, ...)

    return instance
end

---销毁一个class的实例
---@param cls LuaObject
---@param instance LuaObject
function DestroyInstance(cls, instance)
    local dispose = rawget(cls, "__dispose")
    if dispose ~= nil then
        dispose(instance)
    end

    if cls.super then
        DestroyInstance(cls.super, instance)
    end
end

---@param cls LuaObject
---@param super LuaObject
local function inherit(cls, super)
    -- 不能先设置metatable，必须先把父类函数拷贝到本地，然后当前类继续定义的时候才可以覆盖，否则就是把父类定义修改了
    -- 将super中函数缓存到当前cls，减少运行时开销 -- 只拷贝普通函数
    -- 当子类重写函数时，后续的clone会覆盖掉已clone的函数
    -- 缓存到instance性能虽好，但开销巨大，缓存到cls上是一个折中，查询一次元表得到方法
    -- 另外，缓存在instance上时，必须要先拷贝方法才能执行构造函数，构造构造的过程中调用虚方法时将不产生多态行为
    for k, v in pairs(super) do
        if not _ignoreKeys[k] then
            if type(v) == "function" then
                cls[k] = v
            end
        end
    end

    -- 通过设置Class的元表为父类模拟继承
    -- 元表还是需要存在，静态字段必须查元表，实例字段是不需要查元表的，函数也是不需要查询元表（已拷贝）
    if IsClass(super) then
        setmetatable(cls, super)
    else
        setmetatable(cls, { __index = super })
    end
end

---定义一个Class
---模拟Class封装，继承，多态，类型信息，构造函数等
---模拟一个基础的Class需要的信息
---@param clsname string 类名
---@param super table 父类
---@return LuaObject 含Class所需基本信息的Class table
function Class(clsname, super)
    assert(clsname ~= nil and #clsname > 0, "clsname cant be empty")
    assert(type(clsname) == "string", "the type of clsname is error")

    --- 避免重复定义，会导致奇怪的问题
    if class_dic[clsname] ~= nil then
        printError("A class with the same name already exists, clsName: " .. clsname)
    end

    -- 验证super有效性
    if super ~= nil then
        local tt_super = type(super)
        assert(tt_super == "string" or tt_super == "table", tt_super)
        -- 超类可能传了字符串名字
        if tt_super == "string" then
            local tempSuper = RequireClass(super) -- 打印错误需要原始信息
            if tempSuper == nil then
                error("invalid super: " .. tostring(super))
            end
            super = tempSuper
        end
    end
    -- 没有定义超类的统一指向LuaObject
    super = nilToDef(super, LuaObject)

    ---@type LuaObject
    local cls = {}
    cls.super = super
    cls.__cname = clsname
    cls.__ctype = ClassType.CLASS
    cls.__class = cls
    cls.__index = cls -- 查询实例的方法时会从该cls中查询(对自身却无用)

    -- 实现继承
    inherit(cls, super);

    -- 构造函数
    function cls.New(...)
        return NewInstance(cls, ...)
    end

    -- 析构函数
    function cls:Dispose()
        DestroyInstance(cls, self)
    end

    class_dic[clsname] = cls
    return cls
end

---取得对象的类名
---@param objOrClass LuaObject @类
---@return string
function _G.GetClassName(objOrClass)
    if (objOrClass == nil) then
        return nil
    else
        -- 如果覆盖了该字段，我们不予纠正
        return objOrClass.__cname
    end
end

---获取对象的类型
---@param objOrClass LuaObject
---@return LuaObject
function _G.GetClass(objOrClass)
    if (objOrClass == nil) then
        return nil
    else
        -- 如果覆盖了该字段，我们不予纠正
        return objOrClass.__class
    end
end

---@return boolean 如果对象是Class的定义则返回true，否则返回false
function IsClass(obj)
    return obj ~= nil and obj.__ctype == ClassType.CLASS
end

---@return boolean 如果两个对象是同一个类型则返回true
function IsSameClass(obj1, obj2)
    return obj1 ~= nil and GetClass(obj1) == GetClass(obj2)
end

---给定对象是否是指定类的实例
---1.nil必定返回false
---2.非table必定返回false
---@param obj LuaObject @对象
---@param class LuaObject 目标类
---@return boolean @对象是否是某个类的实例
function IsInstanceOf(obj, class)
    -- 参数必须是个class
    assert(IsClass(class), "parameter class is not a class")

    -- nil必定返回false
    if obj == nil then
        return false;
    end
    -- 非table必定返回false
    if type(obj) ~= "table" then
        return false;
    end

    -- 找到当前类型，现在允许子Class是父Class的实例
    local cls
    if obj.__ctype == ClassType.INSTANCE then
        cls = obj.__class
    elseif obj.__ctype == ClassType.CLASS then
        cls = obj
    end

    -- 递归向上搜索
    while cls do
        if cls == class then
            return true
        end
        cls = cls.super
    end

    return false
end

---对应实例封装为不可变
---其实可以通过将inst中的成员拷贝到另一个table实现的，但那样的成本较高，我们现在的实现使用起来还不算复杂，先如此实现
---由于Class的实现尚不完备，不易判断是否是实例字段（也有性能的考虑），因此该包装不是Class
---@param inst LuaObject
---@return LuaObject
function ToImmutableInstance(inst)
    ---@type LuaObject
    local r = setmetatable({}, {
        __index = inst,
        __newindex = function(t, k, v)
            error("this is immutable instance")
        end
    })
    -- 可能要拷贝点特殊的数据
    return r;
end

---根据类名获取类文件，通常是我们根据配置加载类文件
---在lua里，我们更多的是通过require来获得某个类，但require并不支持获得内部类，也就要求我们每一个可被require的类必须是单独的类文件
---这大幅增加了lua脚本数量，如果我们将Class都发布到这里，就可以支持内部类，从而减少脚本数
---@param clsName string 类名
function RequireClass(clsName)
    -- 尝试从已定义class中查询
    local cls = class_dic[clsName]
    if cls == nil then
        if clsName == LuaObject.__cname then
            cls = LuaObject
        else
            -- 尝试加载Lua脚本
            cls = require(clsName)
            if type(cls) ~= "table" or not IsClass(cls) then
                error("class not found exception, clsName: " .. clsName)
            end
        end
    end
    return cls
end