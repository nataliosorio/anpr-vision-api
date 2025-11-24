using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Operational;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Utilities.Qr;

namespace Utilities.Pdf
{
    // Clase principal que define la estructura del PDF (IDocument)
    public class TicketDocument : IDocument
    {
        public TicketData Model { get; }

        public TicketDocument(RegisteredVehiclesDto registeredVehicle)
        {
            string qr = $"Placa:{registeredVehicle.Vehicle}";

            Model = new TicketData
            {
                Plate = registeredVehicle.Vehicle ?? "N/A",
                SlotName = registeredVehicle.Slots ?? "PENDIENTE",
                VehicleType = registeredVehicle.Sector ?? "N/A",   // ← Tipo de Vehículo
                EntryDate = registeredVehicle.EntryDate.GetValueOrDefault(),
                RegisteredVehicleId = (int)registeredVehicle.Id,
                QrImageBytes = QrHelper.GenerateQr(qr)
            };
        }


        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // Método principal donde se define la estructura y el contenido
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    // Define el formato de página para un ticket pequeño y continuo
                    page.Size(210, 400); // Un tamaño pequeño, similar a un recibo
                    page.Margin(10);
                    page.Content().Element(ComposeContent); // Llama al método de diseño
                });
        }

        // Define la estructura visual del ticket
        private void ComposeContent(IContainer container)
        {
            var labelStyle = TextStyle.Default
                .FontSize(8)
                .FontColor("#555");

            var valueStyle = TextStyle.Default
                .FontSize(11)
                .SemiBold()
                .FontColor("#000");

            var titleStyle = TextStyle.Default
                .FontSize(13)
                .SemiBold()
                .FontColor("#00D1FF");

            container
                .Padding(10)
                .Border(1)
                .PaddingHorizontal(8)
                .PaddingVertical(6)
                .Column(col =>
                {
                    col.Spacing(6);

                    // === HEADER ===
                    col.Item().AlignCenter().Text("TICKET DE PARQUEO").Style(titleStyle);
                    col.Item().LineHorizontal(1).LineColor("#00D1FF");

                    // === DATOS (PLACA, TIPO, UBICACIÓN, FECHA) ===
                    col.Item().Column(section =>
                    {
                        section.Spacing(2);

                        section.Item().Text("PLACA").Style(labelStyle);
                        section.Item().Text(Model.Plate)
                            .Style(TextStyle.Default.FontSize(18).Bold().FontColor("#00D1FF"));

                        section.Item().Text("Tipo").Style(labelStyle);
                        section.Item().Text(Model.VehicleType).Style(valueStyle);

                        section.Item().Text("Ubicación").Style(labelStyle);
                        section.Item().Text(Model.SlotName)
                            .Style(valueStyle.FontColor("#10B981"));

                        section.Item().Text("Entrada").Style(labelStyle);

                        var localDate = Model.EntryDate.ToLocalTime();
                        section.Item().Text(localDate.ToString("dd/MM/yyyy HH:mm"))
                            .Style(valueStyle);
                    });

                    // === DIVISOR ===
                    col.Item().PaddingVertical(4)
                        .LineHorizontal(0.5f)
                        .LineColor("#DDDDDD");

                    // === QR EN LA MISMA PÁGINA ===
                    col.Item().AlignCenter()
                        .Container()
                        .PaddingTop(4)
                        .MaxWidth(120)
                        .Image(Model.QrImageBytes);

                    // === MARCA DE AGUA ANPR VISION ===
                    col.Item().AlignCenter()
                        .PaddingTop(3)
                        .Text("ANPR VISION")
                        .Style(TextStyle.Default
                            .FontSize(9)
                            .Italic()
                            .FontColor("#A0A0A0")); // gris suave
                });
        }





    }
}