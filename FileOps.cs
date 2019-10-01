using System.Windows.Forms;

namespace OutputObjAuto
{
    public class FileOps
    {
        // 获取文件路径
        public static string GetFolderPath()
        {
            FolderBrowserDialog newFolderBrowserDialog = new FolderBrowserDialog();
            newFolderBrowserDialog.Description = "Save Obj Geometry To ...";
            newFolderBrowserDialog.ShowNewFolderButton = true;
            var result = newFolderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return newFolderBrowserDialog.SelectedPath;
            }
            else
            {
                return "";
            }
        }
    }
}
