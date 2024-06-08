using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IdbUniversity.Models
{
    public class ValidateFile: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int maxContentLength = 1024 * 1024 * 1024;
            string[] allowedFileExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return false;
            }
            else if (!allowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
            {
                ErrorMessage = "Please upload a file of type: " + string.Join(", ", allowedFileExtensions);
                return false;
            }
            else if (file.ContentLength > maxContentLength)
            {
                ErrorMessage = "Your file is too large, maximum allowed size is: " + (maxContentLength / (1024 * 1024)) + " MB";
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}