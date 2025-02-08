using IngredientsTracker.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IngredientsTracker.ViewModels
{
    partial class IngredientsListVM : BindableObject
    {
        public ObservableCollection<IngredientsModel> Ingredients { get; set; }

        private string _newIngredient;
        public string NewIngredient
        {
            get => _newIngredient;
            set
            {
                _newIngredient = value;
                OnPropertyChanged();
            }
        }

        private string _newIngredientAmount; // Convert to float once focus off and check/validate
        public string NewIngredientAmount
        {
            get => _newIngredientAmount;
            set
            {
                _newIngredientAmount = value;
                OnPropertyChanged();
            }
        }

        public IngredientsListVM() { }
        //public IngredientsListVM()
        //{
        //    Ingredients = new ObservableCollection<IngredientsModel>();
        //    LoadAllIngredients();
        //}

        public async Task LoadAllIngredients()
        {

        }
    }
}
