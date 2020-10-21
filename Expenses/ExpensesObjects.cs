using System;
using Semata.DataStore;
using Semata.DataStore.Util;
using Semata.DataStore.ObjectModel;

namespace Expenses
{
    public sealed partial class ExpensesDataStore : DataStoreObject
    {
        
        string script_ = @"
Add ItemType Activity;
Add ItemType Expense;
Add AttributeType Activity.Description String
{
    ""semata.datastore.objectmodel"" : 
    [
        ""mandatory""
    ]
}
;
Add AttributeType Activity.From Date
{
    ""semata.datastore.objectmodel"" : 
    [
        ""mandatory""
    ]
}
;
Add AttributeType Activity.To Date
{
    ""semata.datastore.objectmodel"" : 
    [
        ""mandatory""
    ]
}
;

Add AttributeType Expense.Amount Decimal
{
    ""semata.datastore.objectmodel"" : 
    [
        ""mandatory""
    ]
}
;
Add AttributeType Expense.Date Date
{
    ""semata.datastore.objectmodel"" : 
    [
        ""mandatory""
    ]
}
;
Add AttributeType Expense.Description String
{
    ""semata.datastore.objectmodel"" : 
    [
        ""mandatory""
    ]
}
;

Add AssociationType Activity.Incurred
{
    ""semata.datastore.objectmodel"" : 
    [
        ""associates_prevent_delete""
    ]
}
to Expense.IncurredOn
{
    ""semata.datastore.objectmodel"" : 
    [
        ""property"",
        ""mandatory""
    ]
}
;

";
        public ItemObjects<Activity> ActivityItems {get; private set;}
        public ItemObjects<Expense> ExpenseItems {get; private set;}
        
        public ExpensesDataStore(PropertyChangedEventDispatcher eventDispatcher) : base(eventDispatcher)
        {
        }
        
        partial void AfterCreate(string path);
        
        public void Create(string path)
        {
            base.CreateInstance(path, "ExpensesDataStore");
            connection_.ExecuteCommands(script_);
            base.CloseInstance();
            AfterCreate(path);
            Open(path);
        }
        
        partial void BeforeOpening(string path);
        
        public void Open(string path)
        {
            BeforeOpening(path);
            base.OpenInstance(path);
            
            //    Activity
            
            ItemObjectDefinition ActivityDefinition = new ItemObjectDefinition();
            ActivityDefinition.AddAttributeProperty("Description", x => (x as Activity).DescriptionProperty);
            ActivityDefinition.AddAttributeProperty("From", x => (x as Activity).FromProperty);
            ActivityDefinition.AddAttributeProperty("To", x => (x as Activity).ToProperty);
            ActivityDefinition.AddAssociation("Incurred", "Incurred", x => (x as Activity).Incurred);
            SetActivator("Activity", (initializer) => new Activity(initializer), ActivityDefinition);
            
            //    Expense
            
            ItemObjectDefinition ExpenseDefinition = new ItemObjectDefinition();
            ExpenseDefinition.AddAttributeProperty("Amount", x => (x as Expense).AmountProperty);
            ExpenseDefinition.AddAttributeProperty("Date", x => (x as Expense).DateProperty);
            ExpenseDefinition.AddAttributeProperty("Description", x => (x as Expense).DescriptionProperty);
            ExpenseDefinition.AddAssociationProperty("IncurredOn", "IncurredOn", x => (x as Expense).IncurredOnProperty);
            SetActivator("Expense", (initializer) => new Expense(initializer), ExpenseDefinition);
            ActivityItems = new ItemObjects<Activity>(connection_.GetItemType("Activity"), this, "ActivityItems");
            ExpenseItems = new ItemObjects<Expense>(connection_.GetItemType("Expense"), this, "ExpenseItems");
        }
        
        public void Close()
        {
            base.CloseInstance();
        }
        
    }
    
    public sealed partial class Activity : ItemObject
    {
        public AttributeProperty<string> DescriptionProperty { get; set;}
        public AttributeProperty<System.DateTime> FromProperty { get; set;}
        public AttributeProperty<System.DateTime> ToProperty { get; set;}
        
        
        public Association<Expense> Incurred { get; private set;}
        
        internal Activity(ItemObjectInitializer initializer) : base(initializer)
        {
            DescriptionProperty = new AttributeProperty<string>(this, "Description", "Description", false, false, (x) => {OnDescriptionChanged(ref x); return x;}, (x) => {OnDescriptionWriting(ref x); return x;});
            FromProperty = new AttributeProperty<System.DateTime>(this, "From", "From", false, false, (x) => {OnFromChanged(ref x); return x;}, (x) => {OnFromWriting(ref x); return x;});
            ToProperty = new AttributeProperty<System.DateTime>(this, "To", "To", false, false, (x) => {OnToChanged(ref x); return x;}, (x) => {OnToWriting(ref x); return x;});
            Incurred = new Association<Expense>(this, "Incurred", "Incurred", "IncurredOn");
            OnInitialize();
        }
        
        public new ExpensesDataStore DataStore => (ExpensesDataStore)base.DataStore;
        
        partial void OnDescriptionChanged(ref object value);
        partial void OnDescriptionWriting(ref object value);
        partial void OnFromChanged(ref object value);
        partial void OnFromWriting(ref object value);
        partial void OnToChanged(ref object value);
        partial void OnToWriting(ref object value);
        partial void OnInitialize();
        partial void OnValidate();
        partial void OnCanDelete();
        partial void OnDeleting();
        partial void OnCreated();
        partial void OnWriting();
        partial void OnWritten();
        
        protected override void OnItemObjectValidate()
        {
            DescriptionProperty.CheckValueSet("");
            FromProperty.CheckValueSet("");
            ToProperty.CheckValueSet("");
            OnValidate();
        }
        
        protected override void OnItemObjectCanDelete()
        {
            Incurred.CheckNoAssociatesExist("");
            OnCanDelete();
        }
        
        protected override void OnItemObjectDeleting()
        {
            OnDeleting();
        }
        
        protected override void OnItemObjectCreated()
        {
            OnCreated();
        }
        
        protected override void OnItemObjectWriting()
        {
            OnWriting();
        }
        
        protected override void OnItemObjectWritten()
        {
            OnWritten();
        }
        
        
        public string Description
        {
            get => (string)DescriptionProperty.Value;
            set => DescriptionProperty.Value = value;
        }
        
        public System.DateTime? From
        {
            get => (System.DateTime?)FromProperty.Value;
            set => FromProperty.Value = value;
        }
        
        public System.DateTime? To
        {
            get => (System.DateTime?)ToProperty.Value;
            set => ToProperty.Value = value;
        }
    }
    
    public sealed partial class Expense : ItemObject
    {
        public AttributeProperty<decimal> AmountProperty { get; set;}
        public AttributeProperty<System.DateTime> DateProperty { get; set;}
        public AttributeProperty<string> DescriptionProperty { get; set;}
        
        public AssociationProperty<Activity> IncurredOnProperty { get; set;}
        
        
        internal Expense(ItemObjectInitializer initializer) : base(initializer)
        {
            AmountProperty = new AttributeProperty<decimal>(this, "Amount", "Amount", false, false, (x) => {OnAmountChanged(ref x); return x;}, (x) => {OnAmountWriting(ref x); return x;});
            DateProperty = new AttributeProperty<System.DateTime>(this, "Date", "Date", false, false, (x) => {OnDateChanged(ref x); return x;}, (x) => {OnDateWriting(ref x); return x;});
            DescriptionProperty = new AttributeProperty<string>(this, "Description", "Description", false, false, (x) => {OnDescriptionChanged(ref x); return x;}, (x) => {OnDescriptionWriting(ref x); return x;});
            IncurredOnProperty = new AssociationProperty<Activity>(this, "IncurredOn", "IncurredOn", false, false, (x) => {OnIncurredOnChanged(ref x); return x;}, (x) => {OnIncurredOnWriting(ref x); return x;}, false);
            OnInitialize();
        }
        
        public new ExpensesDataStore DataStore => (ExpensesDataStore)base.DataStore;
        
        partial void OnAmountChanged(ref object value);
        partial void OnAmountWriting(ref object value);
        partial void OnDateChanged(ref object value);
        partial void OnDateWriting(ref object value);
        partial void OnDescriptionChanged(ref object value);
        partial void OnDescriptionWriting(ref object value);
        partial void OnIncurredOnChanged(ref object value);
        partial void OnIncurredOnWriting(ref object value);
        partial void OnInitialize();
        partial void OnValidate();
        partial void OnCanDelete();
        partial void OnDeleting();
        partial void OnCreated();
        partial void OnWriting();
        partial void OnWritten();
        
        protected override void OnItemObjectValidate()
        {
            AmountProperty.CheckValueSet("");
            DateProperty.CheckValueSet("");
            DescriptionProperty.CheckValueSet("");
            IncurredOnProperty.CheckValueSet("");
            OnValidate();
        }
        
        protected override void OnItemObjectCanDelete()
        {
            OnCanDelete();
        }
        
        protected override void OnItemObjectDeleting()
        {
            OnDeleting();
        }
        
        protected override void OnItemObjectCreated()
        {
            OnCreated();
        }
        
        protected override void OnItemObjectWriting()
        {
            OnWriting();
        }
        
        protected override void OnItemObjectWritten()
        {
            OnWritten();
        }
        
        
        public decimal? Amount
        {
            get => (decimal?)AmountProperty.Value;
            set => AmountProperty.Value = value;
        }
        
        public System.DateTime? Date
        {
            get => (System.DateTime?)DateProperty.Value;
            set => DateProperty.Value = value;
        }
        
        public string Description
        {
            get => (string)DescriptionProperty.Value;
            set => DescriptionProperty.Value = value;
        }
        
        public Activity IncurredOn
        {
            get => (Activity)IncurredOnProperty.Value;
            set => IncurredOnProperty.Value = value;
        }
    }
    
}
