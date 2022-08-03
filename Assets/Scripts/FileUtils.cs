using System;
using System.IO;
using System.Text;

public static class FileUtils
{
    /** <summary>Lua脚本的存储的根路径</summary> */
    private const string LuaPath = "Assets/XLua/{0}.lua";

    /** <summary>Lua脚本不存在的错误提示</summary> */
    private const string ErrorLuaTips = "print(\"<color=#FF0000>Can't require the file:{0}</color>\")\nreturn false";

    /**
     * <param name="path">lua脚本相对路径, 即require的路径，可以用.分割，无需.lua后缀</param>
     */
    public static byte[] RequireLua(ref string path)
    {
        var filePath = string.Format(LuaPath, path.Replace('.', '/'));
        return File.Exists(filePath) ?
            File.ReadAllBytes(filePath) :
            Encoding.UTF8.GetBytes(string.Format(ErrorLuaTips, filePath));
    }
}
