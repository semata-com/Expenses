using System;
using System.Collections.Generic;
using System.ComponentModel;
using Semata.DataStore.ObjectModel;
using Semata.DataStore.ObjectModel.Views;
using Semata.Lazy;
using Semata.EditableData;

namespace Expenses
{
    public partial class ExpensesDataStoreView : INotifyPropertyChanged, INotifyStateChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler StateChanged;
        
        protected Expenses.ExpensesDataStore dataStore_;
        
        LazyValue<ItemObjectViewList<Activity, ActivityView>> activityItems_;
        LazyValue<ItemObjectViewList<Expense, ExpenseView>> expenseItems_;
        
        partial void OnInitialize();
        
        public ExpensesDataStoreView(PropertyChangedEventDispatcher eventDispatcher)
        {
            dataStore_ = new Expenses.ExpensesDataStore(eventDispatcher);
                activityItems_=
                    new LazyValue<ItemObjectViewList<Activity, ActivityView>>(() => 
                        {
                            var list = new ItemObjectViewList<Activity, ActivityView>
                                           (dataStore_.ActivityItems.GetItemObjectCollection()
                                            , (x) => new ActivityView(x, false, false));
                            list.ListChanged += (object sender, EventArgs e) => NotifyPropertyChanged(new PropertyChangedEventArgs("ActivityItems"));
                            return list;
                        });
                expenseItems_=
                    new LazyValue<ItemObjectViewList<Expense, ExpenseView>>(() => 
                        {
                            var list = new ItemObjectViewList<Expense, ExpenseView>
                                           (dataStore_.ExpenseItems.GetItemObjectCollection()
                                            , (x) => new ExpenseView(x, false, false));
                            list.ListChanged += (object sender, EventArgs e) => NotifyPropertyChanged(new PropertyChangedEventArgs("ExpenseItems"));
                            return list;
                        });
        }
        
        protected virtual void NotifyStateChanged(EventArgs args)
        {
            StateChanged?.Invoke(this, args);
        }
        
        protected virtual void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
            NotifyStateChanged(new EventArgs());
        }
        
        public void Open(string path)
        {
            dataStore_.Open(path);
            OnInitialize();
        }
        
        public void Close()
        {
            dataStore_.Close();
        }
        
        public ItemObjectViewList<Activity, ActivityView> ActivityItems => activityItems_.Value;
        
        public ItemObjectViewList<Expense, ExpenseView> ExpenseItems => expenseItems_.Value;
        
}

    public partial class ActivityView : ItemObjectView<Expenses.Activity>
    {
    
        LazyProperty<object> description_;
        LazyProperty<object> from_;
        LazyProperty<object> to_;
        LazyValue<ItemObjectViewList<Expense, ExpenseView>> incurred_;
        
        partial void OnInitialize();
        
        protected override void InitializeValues()
        {
            description_=
                new LazyProperty<object>(this
                    , "Description"
                    , () => Activity.DescriptionProperty.RawValue
                    , (x) => Activity.DescriptionProperty.Value = x
                    , (x) => x != Activity.DescriptionProperty.RawValue
                    , (x) => NotifyPropertyChanged(new PropertyChangedEventArgs("Description")));
        
            from_=
                new LazyProperty<object>(this
                    , "From"
                    , () => Activity.FromProperty.RawValue
                    , (x) => Activity.FromProperty.Value = x
                    , (x) => x != Activity.FromProperty.RawValue
                    , (x) => NotifyPropertyChanged(new PropertyChangedEventArgs("From")));
        
            to_=
                new LazyProperty<object>(this
                    , "To"
                    , () => Activity.ToProperty.RawValue
                    , (x) => Activity.ToProperty.Value = x
                    , (x) => x != Activity.ToProperty.RawValue
                    , (x) => NotifyPropertyChanged(new PropertyChangedEventArgs("To")));
        
            incurred_=
                new LazyValue<ItemObjectViewList<Expense, ExpenseView>>(() =>
                    {
                        var list = new ItemObjectViewList<Expense, ExpenseView>
                                       (Activity.Incurred.GetItemObjectCollection(null, null, null)
                                        , (x) => new ExpenseView(x, false, false));
                        list.ListChanged += (object sender, EventArgs e) => NotifyPropertyChanged(new PropertyChangedEventArgs("Incurred"));
                        return list;
                    });
            OnInitialize();
        }
        
        public ActivityView() : base()
        {
        }
        
        internal ActivityView(Expenses.Activity activity, bool usePropertyChanged, bool writeOnEndEdit) : base(activity, usePropertyChanged, writeOnEndEdit)
        {
        }
        
        internal ActivityView(ActivityView activityView, bool usePropertyChanged, bool writeOnEndEdit) : base(activityView, usePropertyChanged, writeOnEndEdit)
        {
        }
        
        public Activity Activity => ItemObject;
        
        
        public object Description
        {
            get => description_.Value;
            set => description_.Value = value;
        }
        
        public object From
        {
            get => from_.Value;
            set => from_.Value = value;
        }
        
        public object To
        {
            get => to_.Value;
            set => to_.Value = value;
        }
        
        public ItemObjectViewList<Expense, ExpenseView> Incurred => incurred_.Value;
    }
    
    public partial class ExpenseView : ItemObjectView<Expenses.Expense>
    {
    
        LazyProperty<object> amount_;
        LazyProperty<object> date_;
        LazyProperty<object> description_;
        LazyProperty<ActivityView> incurredOn_;
        
        partial void OnInitialize();
        
        protected override void InitializeValues()
        {
            amount_=
                new LazyProperty<object>(this
                    , "Amount"
                    , () => Expense.AmountProperty.RawValue
                    , (x) => Expense.AmountProperty.Value = x
                    , (x) => x != Expense.AmountProperty.RawValue
                    , (x) => NotifyPropertyChanged(new PropertyChangedEventArgs("Amount")));
        
            date_=
                new LazyProperty<object>(this
                    , "Date"
                    , () => Expense.DateProperty.RawValue
                    , (x) => Expense.DateProperty.Value = x
                    , (x) => x != Expense.DateProperty.RawValue
                    , (x) => NotifyPropertyChanged(new PropertyChangedEventArgs("Date")));
        
            description_=
                new LazyProperty<object>(this
                    , "Description"
                    , () => Expense.DescriptionProperty.RawValue
                    , (x) => Expense.DescriptionProperty.Value = x
                    , (x) => x != Expense.DescriptionProperty.RawValue
                    , (x) => NotifyPropertyChanged(new PropertyChangedEventArgs("Description")));
        
            incurredOn_=
                new LazyProperty<ActivityView>(this, "IncurredOn", () =>
                    {
                      return Expense.IncurredOn == null ? null : new ActivityView(Expense.IncurredOn, usePropertyChanged_, writeOnEndEdit_);
                    }
                    , (x) => Expense.IncurredOn = x?.ItemObject
                    , (x) => x?.ItemObject != Expense.IncurredOn
                    , (x) => NotifyPropertyChanged(new PropertyChangedEventArgs("IncurredOn")));
        
            OnInitialize();
        }
        
        public ExpenseView() : base()
        {
        }
        
        internal ExpenseView(Expenses.Expense expense, bool usePropertyChanged, bool writeOnEndEdit) : base(expense, usePropertyChanged, writeOnEndEdit)
        {
        }
        
        internal ExpenseView(ExpenseView expenseView, bool usePropertyChanged, bool writeOnEndEdit) : base(expenseView, usePropertyChanged, writeOnEndEdit)
        {
        }
        
        public Expense Expense => ItemObject;
        
        
        public object Amount
        {
            get => amount_.Value;
            set => amount_.Value = value;
        }
        
        public object Date
        {
            get => date_.Value;
            set => date_.Value = value;
        }
        
        public object Description
        {
            get => description_.Value;
            set => description_.Value = value;
        }
        
        public ActivityView IncurredOn
        {
            get => incurredOn_.Value;
            set => incurredOn_.Value = value;
        }
    }
    
}
