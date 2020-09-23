using System;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using IronPdf;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using IronPdf.Forms;

namespace PdfTest
{
    public class IronPdfSampleFunctions
    {
        [FunctionName("GetPdf_IronPdf")]
        public FileStreamResult GetPdf_IronPdf([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "ssc-form.pdf");

            var pdfContent = GetStandardChoiceForm(path, req.Query["tfn"]);
            return new FileStreamResult(pdfContent, "application/pdf");
        }

        [FunctionName("GetPdf_IronPdf_Filled")]
        public FormField[] GetPdf_IronPdf_Filled([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "ssc-form-filled.pdf");

            return GetFormFields(path);
        }

        private Stream GetStandardChoiceForm(string sourcePath, string tfn)
        {
            var pdfDoc = PdfDocument.FromFile(sourcePath);

            var form = pdfDoc.Form;

            //form.GetFieldByName("2-TFN-fill").Value = (tfn ?? "000000000");
            form.GetFieldByName("2-TFN-sep").Value = tfn ?? "000 000 000";
            pdfDoc.Flatten();

            return pdfDoc.Stream;
        }

        private FormField[] GetFormFields(string sourcePath)
        {
            var pdfDoc = PdfDocument.FromFile(sourcePath);

            return pdfDoc.Form.Fields;
        }
    }
}