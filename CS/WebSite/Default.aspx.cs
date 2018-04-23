using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.XtraScheduler;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web;

public partial class Default : System.Web.UI.Page {
    ASPxSchedulerStorage Storage { get { return ASPxScheduler1.Storage; } }

    protected void Page_Load(object sender, EventArgs e) {
        SetupMappings();
        ResourceFiller.FillResources(this.ASPxScheduler1.Storage, 3);

        ASPxScheduler1.AppointmentDataSource = appointmentDataSource;
        ASPxScheduler1.DataBind();

        ASPxScheduler1.GroupType = SchedulerGroupType.Resource;
    }

    void SetupMappings() {
        ASPxAppointmentMappingInfo mappings = Storage.Appointments.Mappings;
        ASPxAppointmentCustomFieldMappingCollection customFieldMappings = Storage.Appointments.CustomFieldMappings;

        Storage.BeginUpdate();
        try {
            mappings.AppointmentId = "Id";
            mappings.Start = "StartTime";
            mappings.End = "EndTime";
            mappings.Subject = "Subject";
            mappings.AllDay = "AllDay";
            mappings.Description = "Description";
            mappings.Label = "Label";
            mappings.Location = "Location";
            mappings.RecurrenceInfo = "RecurrenceInfo";
            mappings.ReminderInfo = "ReminderInfo";
            mappings.ResourceId = "OwnerId";
            mappings.Status = "Status";
            mappings.Type = "EventType";
            customFieldMappings.Add(new AppointmentCustomFieldMapping("Field1", "Price", FieldValueType.Decimal));
        }
        finally {
            Storage.EndUpdate();
        }
    }

    //Initilazing ObjectDataSource
    protected void appointmentsDataSource_ObjectCreated(object sender, ObjectDataSourceEventArgs e) {
        e.ObjectInstance = new CustomEventDataSource(GetCustomEvents());
    }

    CustomEventList GetCustomEvents() {
        CustomEventList events = Session["ListBoundModeObjects"] as CustomEventList;
        if (events == null) {
            events = new CustomEventList();

            CustomEvent customEvent1 = new CustomEvent();

            customEvent1.Id = customEvent1.GetHashCode();
            customEvent1.StartTime = DateTime.Today.AddHours(1);
            customEvent1.EndTime = DateTime.Today.AddHours(2);
            customEvent1.Subject = "Test event";
            customEvent1.OwnerId = "sbrighton";
            customEvent1.Price = 80;

            events.Add(customEvent1);

            CustomEvent customEvent2 = new CustomEvent();

            customEvent2.Id = customEvent2.GetHashCode();
            customEvent2.StartTime = DateTime.Today.AddHours(4);
            customEvent2.EndTime = DateTime.Today.AddHours(5);
            customEvent2.Subject = "Test event 2";
            customEvent2.OwnerId = "rfischer";
            customEvent2.Price = 120;

            events.Add(customEvent2);

            CustomEvent customEvent3 = new CustomEvent();

            customEvent3.Id = customEvent3.GetHashCode();
            customEvent3.StartTime = DateTime.Today.AddHours(2);
            customEvent3.EndTime = DateTime.Today.AddHours(3);
            customEvent3.Subject = "Test event 3";
            customEvent3.OwnerId = "amiller";
            customEvent3.Price = 95;

            events.Add(customEvent3);

            Session["ListBoundModeObjects"] = events;
        }
        return events;
    }

    // User generated appointment id    
    protected void ASPxScheduler1_AppointmentInserting(object sender, PersistentObjectCancelEventArgs e) {
        SetAppointmentId(sender, e);
    }

    void SetAppointmentId(object sender, PersistentObjectCancelEventArgs e) {
        ASPxSchedulerStorage storage = (ASPxSchedulerStorage)sender;
        Appointment apt = (Appointment)e.Object;
        storage.SetAppointmentId(apt, apt.GetHashCode());
    }

    protected void ASPxScheduler1_InitAppointmentDisplayText(object sender, AppointmentDisplayTextEventArgs e) {
        e.Description = string.Format("Price: {0:c}", e.Appointment.CustomFields["Field1"]);
    }

    protected void ASPxScheduler1_BeforeExecuteCallbackCommand(object sender, SchedulerCallbackCommandEventArgs e) {
        ASPxScheduler scheduler = (ASPxScheduler)sender;

        if (Convert.ToDecimal(scheduler.SelectedAppointments[0].CustomFields["Field1"]) > 100m) {
            if (e.CommandId == SchedulerCallbackCommandId.MenuAppointment)
                e.Command = new CustomMenuAppointmentCallbackCommand(scheduler);
            if (e.CommandId == SchedulerCallbackCommandId.AppointmentsChange)
                e.Command = new CustomAppointmentsChangeCallbackCommand(scheduler);
        }
    }
}

public class CustomMenuAppointmentCallbackCommand : MenuAppointmentCallbackCommand {
    private bool currentCommandProhibited;

    public CustomMenuAppointmentCallbackCommand(ASPxScheduler control)
        : base(control) {

    }

    protected override void ParseParameters(string parameters) {
        currentCommandProhibited = (parameters == "OpenAppointment" || parameters == "DeleteAppointment"
            || parameters.Contains("LabelSubMenu") || parameters.Contains("StatusSubMenu"));

        base.ParseParameters(parameters);
    }

    protected override void ExecuteCore() {
        if (currentCommandProhibited) {
            Control.CustomJSProperties += new CustomJSPropertiesEventHandler(ASPxScheduler1_CustomJSProperties);
        }
        else {
            base.ExecuteCore();
        }
    }

    private void ASPxScheduler1_CustomJSProperties(object sender, CustomJSPropertiesEventArgs e) {
        e.Properties.Add("cpWarning", "The currently selected appointment cannot be modified because of its Price value.");
    }
}

public class CustomAppointmentsChangeCallbackCommand : AppointmentsChangeCommand {
    public CustomAppointmentsChangeCallbackCommand(ASPxScheduler control)
        : base(control) {

    }

    protected override void ExecuteCore() {
        Control.CustomJSProperties += new CustomJSPropertiesEventHandler(ASPxScheduler1_CustomJSProperties);
    }

    private void ASPxScheduler1_CustomJSProperties(object sender, CustomJSPropertiesEventArgs e) {
        e.Properties.Add("cpWarning", "The currently selected appointment cannot be modified because of its Price value.");
    }
}