using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteClassLibrary.Helper
{
    public class FolderHelper
    {
        public string GetFolderName(string folderPath)
        {
            if (!String.IsNullOrEmpty(folderPath))
            {
                var folders = folderPath.Split('.');
                if (folders.Length > 0 && folders[1]!=null)
                    return folders[1];
            }

            throw new Exception(CodeHelper.UnableToFindFolder);
        }

    }
}
