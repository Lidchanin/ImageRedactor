using Xamarin.Forms;

namespace ImageRedactor.Services
{
    public interface IViewSnapshotService
    {
        void MakeViewSnapshotAndSave(View view, string snapshotName);
    }
}