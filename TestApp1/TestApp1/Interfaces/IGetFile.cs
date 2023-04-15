using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp1.Interfaces
{
    public interface IGetFile
    {
        string GetImagePath();
        string GetAudioFiles();

        string GetErrorFiles();
    }
}
