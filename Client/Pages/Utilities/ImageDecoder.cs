using FinanceApp.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace FinanceApp.Client.Pages.Utilities;

public class ImageDecoder : ComponentBase
{
    protected static string ConvertToBase64(LogoDto logo)
    {
        if (logo.Format == "svg") logo.Format = "svg+xml";
        return $"data:image/{logo.Format};base64,{Convert.ToBase64String(logo.Data)}";
    }
}