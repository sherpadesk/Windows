using SherpaDesk.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SherpaDesk.Models.Request
{
    public class FileRequest : PostRequest, IPath, IDisposable
    {
        public const string IMAGE_MEDIA_TYPE = "image/jpeg";

        private MultipartFormDataContent _fileContent;
        private ByteArrayContent _streamContent;

        public static FileRequest Create(string ticketKey, int? postId)
        {
            if (postId.HasValue)
            {
                return new FileRequest(ticketKey, postId.Value);
            }
            else
            {
                return new FileRequest(ticketKey);
            }
        }

        public FileRequest(string ticketKey)
        {
            _fileContent = new MultipartFormDataContent();
            this.Path = string.Format("?ticket={0}", ticketKey);
        }

        public FileRequest(string ticketKey, int postId)
        {
            _fileContent = new MultipartFormDataContent();
            this.Path = string.Format("?ticket={0}&post_id={1}", ticketKey, postId);
        }

        [Details]
        [Required(ErrorMessage = "Ticket key is required")]
        public string Path { get; set; }

        public async Task Add(StorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                var fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }

                _streamContent = new ByteArrayContent(fileBytes);
                MediaTypeHeaderValue mediaType;
                if (!MediaTypeHeaderValue.TryParse(file.ContentType, out mediaType))
                {
                    mediaType = MediaTypeHeaderValue.Parse(IMAGE_MEDIA_TYPE);
                }
                _streamContent.Headers.ContentType = mediaType;

                _fileContent.Add(_streamContent, file.DisplayName, file.Name);
            }
        }

        public override HttpContent GetContent()
        {
            return _fileContent;
        }

        ~FileRequest()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_streamContent != null)
                {
                    _streamContent.Dispose();
                    _streamContent = null;
                }
                if (_fileContent != null)
                {
                    _fileContent.Dispose();
                    _fileContent = null;
                }
            }
        }
    }
}
