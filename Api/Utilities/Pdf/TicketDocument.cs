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
            string qr = $"Entry:{registeredVehicle.Id}|Plate:{registeredVehicle.Vehicle}";

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
                    page.Size(PageSizes.A7); // Un tamaño pequeño, similar a un recibo
                    page.Margin(10);
                    page.Content().Element(ComposeContent); // Llama al método de diseño
                });
        }

        // Define la estructura visual del ticket
        private void ComposeContent(IContainer container)
        {
            var labelStyle = TextStyle.Default
                .FontSize(9)
                .FontColor(Colors.Grey.Darken2);

            var valueStyle = TextStyle.Default
                .FontSize(12)
                .SemiBold()
                .FontColor(Colors.Black);

            container.Padding(18).Column(col =>
            {
                col.Spacing(14);

                // HEADER
                col.Item().Column(header =>
                {
                    header.Spacing(4);

                    header.Item().AlignCenter().Text("TICKET DE PARQUEO")
                        .Style(TextStyle.Default.FontSize(14).SemiBold().FontColor("#00D1FF"));

                    header.Item().LineHorizontal(1).LineColor("#00D1FF");
                });

                // PLACA
                col.Item().Column(info =>
                {
                    info.Spacing(2);

                    info.Item().Text("PLACA").Style(labelStyle);
                    info.Item().Text(Model.Plate)
                        .Style(TextStyle.Default.FontSize(22).ExtraBold().FontColor("#00D1FF"));
                });

                // TIPO VEHÍCULO
                col.Item().Column(info =>
                {
                    info.Spacing(1);
                    info.Item().Text("Tipo de vehículo").Style(labelStyle);
                    info.Item().Text(Model.VehicleType).Style(valueStyle);
                });

                // SLOT
                col.Item().Column(info =>
                {
                    info.Spacing(1);
                    info.Item().Text("Slot asignado").Style(labelStyle);
                    info.Item().Text(Model.SlotName)
                        .Style(valueStyle.FontColor("#34D399")); // verde ANPR Vision
                });

                // ENTRADA
                col.Item().Column(info =>
                {
                    info.Spacing(1);
                    info.Item().Text("Entrada").Style(labelStyle);
                    info.Item().Text(Model.EntryDate.ToString("dd/MM/yyyy HH:mm"))
                        .Style(valueStyle);
                });

                // Separador suave tipo neumorfismo
                col.Item().PaddingVertical(10).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                // QR CENTRADO
                col.Item().AlignCenter().Container().MaxWidth(120).Image(Model.QrImageBytes);

                // ID
                //col.Item().AlignCenter().PaddingTop(6)
                //    .Text($"ID: {Model.RegisteredVehicleId}")
                //    .Style(TextStyle.Default.FontSize(10).FontColor(Colors.Grey.Darken1));
            });
        }


    }
}