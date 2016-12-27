using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NaughtyKid.MyEnum;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace NaughtyKid.Extension
{
    public static class StringExtend
    {
        /// <summary>
        /// 字符串生成MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Hash(this string input)
        {
            string pwd = string.Empty;
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符

                pwd = pwd + s[i].ToString("x2");

            }
            return pwd;
        }

        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="sfile">源文件路径</param>
        /// <param name="dfile">目的文件路径</param>
        /// <param name="sectSize">传输大小 1048576</param>
        /// <param name="type">文件扩展名</param>
        /// <param name="delect">是否删除</param>
        /// <returns>是否成功</returns>
        public static bool CopyFile(this string sfile, string dfile, int sectSize, string type, bool delect)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(dfile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dfile));
                FileStream fileToCreate = new FileStream(dfile, FileMode.Create); //创建目的文件，如果已存在将被覆盖
                fileToCreate.Close(); //关闭所有资源
                fileToCreate.Dispose(); //释放所有资源
                FileStream formerOpen = new FileStream(sfile, FileMode.Open, FileAccess.Read); //以只读方式打开源文件
                FileStream toFileOpen = new FileStream(dfile, FileMode.Append, FileAccess.Write); //以写方式打开目的文件
                if (sectSize < formerOpen.Length) //如果分段拷贝，即每次拷贝内容小于文件总长度
                {
                    byte[] buffer = new byte[sectSize]; //根据传输的大小，定义一个字节数组
                    int copied = 0; //记录传输的大小

                    while (copied <= ((int) formerOpen.Length - sectSize)) //拷贝主体部分
                    {
                        var fileSize = formerOpen.Read(buffer, 0, sectSize); //要拷贝的文件的大小
                        formerOpen.Flush(); //清空缓存
                        toFileOpen.Write(buffer, 0, sectSize); //向目的文件写入字节
                        toFileOpen.Flush(); //清空缓存
                        toFileOpen.Position = formerOpen.Position; //使源文件和目的文件流的位置相同
                        copied += fileSize; //记录已拷贝的大小

                    }
                    int left = (int) formerOpen.Length - copied; //获取剩余大小
                    formerOpen.Read(buffer, 0, left); //读取剩余的字节
                    formerOpen.Flush(); //清空缓存
                    toFileOpen.Write(buffer, 0, left); //写入剩余的部分
                    toFileOpen.Flush(); //清空缓存
                }
                else //如果整体拷贝，即每次拷贝内容大于文件总长度
                {
                    byte[] buffer = new byte[formerOpen.Length]; //获取文件的大小
                    formerOpen.Read(buffer, 0, (int) formerOpen.Length); //读取源文件的字节
                    formerOpen.Flush(); //清空缓存
                    toFileOpen.Write(buffer, 0, (int) formerOpen.Length); //写放字节
                    toFileOpen.Flush(); //清空缓存
                }
                formerOpen.Close(); //释放所有资源
                formerOpen.Dispose();
                toFileOpen.Close(); //释放所有资源
                toFileOpen.Dispose();

                string a = Path.GetDirectoryName(dfile) + "\\" + Path.GetFileNameWithoutExtension(dfile) + type;

                File.Move(dfile, a);

                if (delect) File.Delete(sfile);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="sfile">源文件路径</param>
        /// <param name="dfile">目的文件路径</param>
        /// <param name="sectSize">传输大小 1048576</param>
        /// <param name="type">文件扩展名</param>
        /// <param name="delect">是否删除</param>
        /// <returns>是否成功</returns>
        public static bool CopyFile(this string sfile, string dfile, int sectSize, string type, bool delect, bool iscover)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(dfile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dfile));
                FileStream fileToCreate = new FileStream(dfile, FileMode.Create);//创建目的文件，如果已存在将被覆盖
                fileToCreate.Close();//关闭所有资源
                fileToCreate.Dispose();//释放所有资源
                FileStream formerOpen = new FileStream(sfile, FileMode.Open, FileAccess.Read);//以只读方式打开源文件
                FileStream toFileOpen = new FileStream(dfile, FileMode.Append, FileAccess.Write);//以写方式打开目的文件
                if (sectSize < formerOpen.Length)//如果分段拷贝，即每次拷贝内容小于文件总长度
                {
                    byte[] buffer = new byte[sectSize];//根据传输的大小，定义一个字节数组
                    int copied = 0;//记录传输的大小

                    while (copied <= ((int)formerOpen.Length - sectSize))//拷贝主体部分
                    {
                        var fileSize = formerOpen.Read(buffer, 0, sectSize);//要拷贝的文件的大小
                        formerOpen.Flush();//清空缓存
                        toFileOpen.Write(buffer, 0, sectSize);//向目的文件写入字节
                        toFileOpen.Flush();//清空缓存
                        toFileOpen.Position = formerOpen.Position;//使源文件和目的文件流的位置相同
                        copied += fileSize;//记录已拷贝的大小

                    }
                    int left = (int)formerOpen.Length - copied;//获取剩余大小
                    formerOpen.Read(buffer, 0, left);//读取剩余的字节
                    formerOpen.Flush();//清空缓存
                    toFileOpen.Write(buffer, 0, left);//写入剩余的部分
                    toFileOpen.Flush();//清空缓存
                }
                else//如果整体拷贝，即每次拷贝内容大于文件总长度
                {
                    byte[] buffer = new byte[formerOpen.Length];//获取文件的大小
                    formerOpen.Read(buffer, 0, (int)formerOpen.Length);//读取源文件的字节
                    formerOpen.Flush();//清空缓存
                    toFileOpen.Write(buffer, 0, (int)formerOpen.Length);//写放字节
                    toFileOpen.Flush();//清空缓存
                }
                formerOpen.Close();//释放所有资源
                formerOpen.Dispose();
                toFileOpen.Close();//释放所有资源
                toFileOpen.Dispose();

                var a = Path.GetDirectoryName(dfile) + "\\" + Path.GetFileNameWithoutExtension(dfile) + type;

                if(File.Exists(a) && iscover)File.Delete(a);

                File.Move(dfile, a);

                if (delect) File.Delete(sfile);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="sfile">源文件路径</param>
        /// <param name="dfile">目的文件路径</param>
        /// <param name="sectSize">传输大小 1048576</param>
        /// <param name="type">文件扩展名</param>
        /// <param name="index">进度值</param>
        /// <returns>是否成功</returns>
        public static bool CopyFile(this string sfile, string dfile, int sectSize, string type, int index)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(dfile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dfile));
                FileStream fileToCreate = new FileStream(dfile, FileMode.Create);//创建目的文件，如果已存在将被覆盖
                fileToCreate.Close();//关闭所有资源
                fileToCreate.Dispose();//释放所有资源
                FileStream formerOpen = new FileStream(sfile, FileMode.Open, FileAccess.Read);//以只读方式打开源文件
                FileStream toFileOpen = new FileStream(dfile, FileMode.Append, FileAccess.Write);//以写方式打开目的文件
                if (sectSize < formerOpen.Length)//如果分段拷贝，即每次拷贝内容小于文件总长度
                {
                    byte[] buffer = new byte[sectSize];//根据传输的大小，定义一个字节数组
                    int copied = 0;//记录传输的大小

                    while (copied <= ((int)formerOpen.Length - sectSize))//拷贝主体部分
                    {
                        var fileSize = formerOpen.Read(buffer, 0, sectSize);//要拷贝的文件的大小
                        formerOpen.Flush();//清空缓存
                        toFileOpen.Write(buffer, 0, sectSize);//向目的文件写入字节
                        toFileOpen.Flush();//清空缓存
                        toFileOpen.Position = formerOpen.Position;//使源文件和目的文件流的位置相同
                        copied += fileSize;//记录已拷贝的大小

                    }
                    int left = (int)formerOpen.Length - copied;//获取剩余大小
                    formerOpen.Read(buffer, 0, left);//读取剩余的字节
                    formerOpen.Flush();//清空缓存
                    toFileOpen.Write(buffer, 0, left);//写入剩余的部分
                    toFileOpen.Flush();//清空缓存
                }
                else//如果整体拷贝，即每次拷贝内容大于文件总长度
                {
                    byte[] buffer = new byte[formerOpen.Length];//获取文件的大小
                    formerOpen.Read(buffer, 0, (int)formerOpen.Length);//读取源文件的字节
                    formerOpen.Flush();//清空缓存
                    toFileOpen.Write(buffer, 0, (int)formerOpen.Length);//写放字节
                    toFileOpen.Flush();//清空缓存
                }
                formerOpen.Dispose();
                formerOpen.Close();//释放所有资源
                toFileOpen.Dispose();
                toFileOpen.Close();//释放所有资源

                string a = Path.GetDirectoryName(dfile) + "\\" + Path.GetFileNameWithoutExtension(dfile) + type;
                if (File.Exists(a)) File.Delete(a);
                File.Move(dfile, a);
                File.Delete(sfile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// 检测文件是否可以使用true为可以使用false为不可使用
        /// 
        /// <param name="file">文件名</param>
        /// <returns></returns>
        public static bool Lockft(this string file)
        {
            try
            {
                var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
                fs.Close();
                fs.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

       /// <summary>
       /// 将字符保存到文件中
       /// </summary>
       /// <param name="meg">内容</param>
       /// <param name="path">文件路径</param>
       /// <returns></returns>
        public static bool WriteMessage(this string meg, string path)
        {
            try
            {
                FileStream log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position = log.Length;
                StreamWriter sw = new StreamWriter(log);
                sw.WriteLine(meg);
                sw.Close();
                sw.Dispose();
                log.Close();
                log.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 启动应用
        /// </summary>
        /// <param name="path">应用程序路径</param>
        /// <param name="repeat">是否重复启动</param>
        /// <returns></returns>
        public static bool ExeStart(this string path, bool repeat)
        {
            try
            {
                if (!repeat)
                {
                    var pr = System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path));
                    if (pr.Length > 0) return false;
                }

                //创建启动对象  
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = path,
                    Verb = "runas",

                };
                //设置启动动作,确保以管理员身份运行  

                System.Diagnostics.Process.Start(startInfo);

                return true;
            }
            catch (Exception ex)
            {
                Error.ErrorHelper.ErrorPutting(ex, "ExeStart");
                return false;
            }

        }

        /// <summary>
        /// 启动应用
        /// </summary>
        /// <param name="path">应用程序路径</param>
        /// <param name="repeat">是否重复启动</param>
        /// <param name="arguments">启动参数</param>
        /// <returns></returns>
        public static bool ExeStart(this string path, bool repeat, string arguments)
        {
            try
            {
                if (!repeat)
                {
                    var pr = System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path));
                    if (pr.Length > 0) return false;
                }
           
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = path,
                    Verb = "runas",
                    Arguments = arguments
                };

                System.Diagnostics.Process.Start(startInfo);

                return true;
            }
            catch (Exception ex)
            {             
                Error.ErrorHelper.ErrorPutting(ex, "ExeStart");
                return false;
            }

        }

        /// <summary>
        /// 判断程序是否启动
        /// </summary>
        /// <param name="exeName"></param>
        /// <returns></returns>
        public static bool IsExeStart(this string exeName)
        {
            var pr = System.Diagnostics.Process.GetProcessesByName(exeName);
            return pr.Length > 0;
        }

        /// <summary>
        /// 生成程序快捷方式
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CreateLnk(this string fileName)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\Link\\" + fileName.Substring(fileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1) + ".lnk") == false)
            {
                //实例化WshShell对象 
                WshShell shell = new WshShell();

                //通过该对象的 CreateShortcut 方法来创建 IWshShortcut 接口的实例对象 
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(
                    Directory.GetCurrentDirectory() + "\\Link\\" + fileName.Substring(fileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1) + ".lnk");

                //设置快捷方式的目标所在的位置(源程序完整路径) 
                shortcut.TargetPath = fileName;

                //应用程序的工作目录 
                //当用户没有指定一个具体的目录时，快捷方式的目标应用程序将使用该属性所指定的目录来装载或保存文件。 
                shortcut.WorkingDirectory = Path.GetDirectoryName(fileName);

                //目标应用程序窗口类型(1.Normal window普通窗口,3.Maximized最大化窗口,7.Minimized最小化) 
                shortcut.WindowStyle = 1;

                //快捷方式的描述 
                shortcut.Description = "ChinaDforce YanMang";

                //保存快捷方式 
                shortcut.Save();

                return Directory.GetCurrentDirectory() + "\\Link\\" + fileName.Substring(fileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1) + ".lnk";
            }
            else
            {
                return Directory.GetCurrentDirectory() + "\\Link\\" + fileName.Substring(fileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1) + ".lnk";
            }

        }
        /// <summary>
        /// 生成快捷方式
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dsPath">生成快方式的路径</param>
        /// <param name="description">生成快方式的描述</param>
        /// <returns></returns>
        public static string CreateLnk(this string fileName, string dsPath, string description)
        {

            string tempdsPath;
            if (!File.Exists(dsPath))
            {
                tempdsPath = dsPath;
            }
            else
            {
                tempdsPath = dsPath + DateTime.Now.ToString("hhmmss");
            }


            //实例化WshShell对象 
            WshShell shell = new WshShell();

            //通过该对象的 CreateShortcut 方法来创建 IWshShortcut 接口的实例对象 
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(
               tempdsPath);

            //设置快捷方式的目标所在的位置(源程序完整路径) 
            shortcut.TargetPath = fileName;

            //应用程序的工作目录 
            //当用户没有指定一个具体的目录时，快捷方式的目标应用程序将使用该属性所指定的目录来装载或保存文件。 
            shortcut.WorkingDirectory = Path.GetDirectoryName(fileName);

            //目标应用程序窗口类型(1.Normal window普通窗口,3.Maximized最大化窗口,7.Minimized最小化) 
            shortcut.WindowStyle = 1;

            //快捷方式的描述 
            shortcut.Description = description;

            //保存快捷方式 
            shortcut.Save();

            return tempdsPath;

        }

        /// <summary>
        /// 文件夹选取器
        /// </summary>
        /// <param name="dirPath">默认目录</param>
        /// <param name="description">显示标题</param>
        /// <returns>目录地址</returns>
        public static string SelectDirectory(this string dirPath, string description)
        {
            var folderBrowserDialog =
                new FolderBrowserDialog
                {
                    Description = description,
                    ShowNewFolderButton = true,
                    SelectedPath = dirPath
                };

            var result = folderBrowserDialog.ShowDialog();

            return result == DialogResult.OK ? folderBrowserDialog.SelectedPath : string.Empty;
        }

        /// <summary>
        /// 文件选取器
        /// </summary>
        /// <param name="dirPath">默认目录</param>
        /// <param name="description">显示标题</param>
        /// <param name="type">文件类型</param>
        /// <param name="multiselect">允许多选</param>
        /// <returns></returns>
        public static string[] SelectFiles(this string dirPath, string description, string type, bool multiselect)
        {
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = dirPath,
                Title = description,
                Filter = type,
                Multiselect = multiselect
            };
            var result = fileDialog.ShowDialog();

            return result == DialogResult.OK ? fileDialog.FileNames : null;
        }

        /// <summary>
        /// 文件另存为
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="fileName">保存的文件名字</param>
        /// <returns></returns>
        public static string SaveFileSelectDirectory(this string dirPath, string fileName,string type)
        {
            var savedialog = new SaveFileDialog()
            {
                InitialDirectory = dirPath,
                Filter = type,   
                FileName =  fileName
            };

            var result = savedialog.ShowDialog();

            return result == DialogResult.OK ?savedialog.FileName : "";
        }

        /// <summary>
        /// 数据长度转换
        /// </summary>
        /// <param name="len">Bytes</param>
        /// <returns></returns>
        public static string ConvertBytes(this float len,ByteMeasurementEum nm)
        {
            var sizetype = nm;
            var offset = 1024;
             switch (nm)
            {
                case ByteMeasurementEum.KB:
                    len = len / offset;
                    break;
                case ByteMeasurementEum.MB:
                    offset = offset >> 1;
                    len = len / offset;
                    break;
                case ByteMeasurementEum.GB:
                    offset = offset >> 2;
                    len = len / offset;
                    break;
                case ByteMeasurementEum.TB:
                    offset = offset >> 3;
                    len = len / offset;
                    break;
            }

            return String.Format("{0:0.##} {1}", len, sizetype);
        }

        /// <summary>
        /// 获取文件夹中指定类型文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="option">搜索范围</param>
        /// <param name="searchpattern">文件类型</param>
        /// <param name="split">分割符默认|</param>
        /// <returns></returns>
        public static List<string> GetFiles(this string path, SearchOption option, string searchpattern = "*.*" ,char split ='|')
        {
            var Searchpattern = searchpattern.Split(split);

             var files = Searchpattern.SelectMany(end => Directory.GetFiles(path, end, option)).ToList();

            return files.ToList();
        }

        /// <summary>
        /// 删除文件夹（及文件夹下所有子文件夹和文件）
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        public static void DeleteFolder(this string directoryPath)
        {
            try
            {
                foreach (var d in Directory.GetFileSystemEntries(directoryPath))
                {

                    if (File.Exists(d))
                    {
                        var fi = new FileInfo(d);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(d);     //删除文件   
                    }
                    else
                        DeleteFolder(d);    //删除文件夹          

                }
                Directory.Delete(directoryPath);    //删除空文件夹
            }
            catch (Exception ex)
            {
                Error.ErrorHelper.ErrorPutting(ex, "DeleteFolder");
            }
           
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>true or false</returns>
        public static bool FileDelect(this string filePath)
        {
            try
            {
                if(File.Exists(filePath))
                {
                    new FileInfo(filePath).Delete();
                }
                return true;
            }
            catch(Exception ex)
            {
                Error.ErrorHelper.ErrorPutting(ex, "FileDelect");
                return false;
            }
           
        }

        /// <summary>
        /// 创建目录存在或创建成功均返回true
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool CreatDirectory(this string directoryPath)
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
                return true;
            }
            catch (Exception ex)
            {
                Error.ErrorHelper.ErrorPutting(ex, "CreatDirectory");
                return false;
            }
        }
    }
}
