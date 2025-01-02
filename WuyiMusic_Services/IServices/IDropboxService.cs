using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuyiMusic_Services.IServices
{
    public interface IDropboxService
    {
        Task UploadFileAsync(Stream fileStream, string fileName);
    }
}
