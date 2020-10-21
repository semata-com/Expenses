using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semata.DataStore.ObjectModel;
using Semata.DataStore.ObjectModel.Views;
using Semata.Lazy;
using Semata.EditableData;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Expenses
{
    public partial class ExpensesDataStoreView
    {
        LazyValue<ItemObjectViewList<Activity, ActivityView>> editableActivityItems_;

        partial void OnInitialize()
        {
            editableActivityItems_ =
                new LazyValue<ItemObjectViewList<Activity, ActivityView>>(() =>
                {
                    var list = new ItemObjectViewList<Activity, ActivityView>
                                       (dataStore_.ActivityItems.GetItemObjectCollection()
                                        , (x) => new ActivityView(x, true, true));
                    
                    list.ListChanged += (object sender, EventArgs e) => NotifyPropertyChanged(new PropertyChangedEventArgs("EditableActivityItems"));
                    return list;
                });
        }

        public ItemObjectViewList<Activity, ActivityView> EditableActivityItems => editableActivityItems_.Value;

    }

    public partial class ActivityView
    {

        LazyValue<ItemObjectViewList<Expense, ExpenseView>> expensesIncurred_;
        partial void OnInitialize()
        {
            expensesIncurred_ =
                new LazyValue<ItemObjectViewList<Expense, ExpenseView>>(() =>
                {
                    var list = new ItemObjectViewList<Expense, ExpenseView>
                                       (ItemObject.Incurred.GetItemObjectCollection()
                                        , (x) => new ExpenseView(x, true, true));

                    list.ListChanged += (object sender, EventArgs e) => NotifyPropertyChanged(new PropertyChangedEventArgs("ExpensesIncurred"));
                    return list;

                });

        }

        public ItemObjectViewList<Expense, ExpenseView> ExpensesIncurred => expensesIncurred_.Value;

        // Work around to prevent initialize validation on BeginEdit - only a problem with new Items
        //                         vvvv

        bool firstValidate_ = false;
        public override void BeginEdit()
        {
            base.BeginEdit();
            firstValidate_ = true;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!firstValidate_)
            {
                return base.Validate(validationContext);
            }
            else
            {
                firstValidate_ = false;
                return Enumerable.Empty<ValidationResult>();
            }
        }

        //                         ^^^^

    }

    public partial class ExpenseView
    {

        // Work around to prevent initialize validation on BeginEdit - only a problem with new Items
        //                         vvvv

        bool firstValidate_ = false;
        public override void BeginEdit()
        {
            base.BeginEdit();
            firstValidate_ = true;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!firstValidate_)
            {
                return base.Validate(validationContext);
            }
            else
            {
                firstValidate_ = false;
                return Enumerable.Empty<ValidationResult>();
            }
        }

        //                         ^^^^

    }
}
