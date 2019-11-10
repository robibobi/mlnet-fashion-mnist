using System;
using System.Threading.Tasks;

namespace Tcoc.FashionMnist.Services
{
    interface IDialogService
    {
        Task<string> ShowModelNameDialog(string suggestedfileName);

        Task ShowProgressDialog(string header, Action<Action<string>> actionDelegate);

    }
}
