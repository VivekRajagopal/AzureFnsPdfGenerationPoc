using System;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using iText.IO.Source;
using iText.Kernel.Pdf;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using iText.Layout;
using iText.Forms;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using iText.Forms.Fields;
using System.Collections.Generic;

namespace PdfTest
{
    public class HtmlToPdfConverter
    {
        [FunctionName("GetPdf_iText")]
        public FileStreamResult GetPdf_iText([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ExecutionContext context)
        {
            var path = System.IO.Path.Combine(context.FunctionAppDirectory, "ssc-form.pdf");

            //var response = new HttpResponseMessage(HttpStatusCode.OK);
            var pdfContentBytes = GetStandardChoiceForm(path, req.Query["tfn"]);
            //response.Content = new ByteArrayContent(pdfContentBytes);
            //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "my-ssc-form.pdf" };
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            return new FileStreamResult(pdfContentBytes, "application/pdf");
        }

        [FunctionName("GetPdf_iText_Filled")]
        public IDictionary<string, PdfFormField> GetPdf_iText_Filled([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "ssc-form-filled.pdf");

            return GetFormFields(path);
        }

        private Stream GetStandardChoiceForm(string sourcePath, string tfn)
        {
            using (var stream = new MemoryStream())
            {
                var baos = new ByteArrayOutputStream();
                var pdfDoc = new PdfDocument(new PdfReader(sourcePath), new PdfWriter(stream));

                //var doc = new Document(pdfDoc);
                var form = PdfAcroForm.GetAcroForm(pdfDoc, false);

                var fields = form.GetFormFields();
                fields["2-TFN-fill"]?.SetValue(tfn ?? "undefined");

                form.FlattenFields();

                pdfDoc.Close();
                baos.WriteTo(stream);

                return stream;
            }
        }

        private IDictionary<string, PdfFormField> GetFormFields(string sourcePath)
        {
            var baos = new ByteArrayOutputStream();

            var pdfDoc = new PdfDocument(new PdfReader(sourcePath), new PdfWriter(baos));
            var form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            var fields = form.GetFormFields();
            fields["2-TFN-fill"]?.SetValue("undefined");
            form.FlattenFields();

            //pdfDoc.Close();

            return form.GetFormFields();
        }
    }
}