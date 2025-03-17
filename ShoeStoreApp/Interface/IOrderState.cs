using ShoeStoreApp.Models;

namespace ShoeStoreApp.Interface
{
    public interface IOrderState
    {
        void Process(string orderSelectedId);
    }
}
