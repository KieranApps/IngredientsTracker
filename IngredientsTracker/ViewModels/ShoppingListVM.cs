using IngredientsTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngredientsTracker.ViewModels
{
    public partial class ShoppingListVM : BindableObject
    {

        private readonly ApiService _api;

        public ShoppingListVM() { }
        public ShoppingListVM(ApiService api)
        {
            _api = api;
        }
    }
}
