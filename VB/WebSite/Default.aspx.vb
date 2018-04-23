Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports DevExpress.XtraScheduler
Imports DevExpress.Web.ASPxScheduler
Imports DevExpress.Web.ASPxScheduler.Internal
Imports DevExpress.Web.ASPxClasses

Partial Public Class [Default]
	Inherits System.Web.UI.Page
	Private ReadOnly Property Storage() As ASPxSchedulerStorage
		Get
			Return ASPxScheduler1.Storage
		End Get
	End Property

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		SetupMappings()
		ResourceFiller.FillResources(Me.ASPxScheduler1.Storage, 3)

		ASPxScheduler1.AppointmentDataSource = appointmentDataSource
		ASPxScheduler1.DataBind()

		ASPxScheduler1.GroupType = SchedulerGroupType.Resource
	End Sub

	Private Sub SetupMappings()
		Dim mappings As ASPxAppointmentMappingInfo = Storage.Appointments.Mappings
		Dim customFieldMappings As ASPxAppointmentCustomFieldMappingCollection = Storage.Appointments.CustomFieldMappings

		Storage.BeginUpdate()
		Try
			mappings.AppointmentId = "Id"
			mappings.Start = "StartTime"
			mappings.End = "EndTime"
			mappings.Subject = "Subject"
			mappings.AllDay = "AllDay"
			mappings.Description = "Description"
			mappings.Label = "Label"
			mappings.Location = "Location"
			mappings.RecurrenceInfo = "RecurrenceInfo"
			mappings.ReminderInfo = "ReminderInfo"
			mappings.ResourceId = "OwnerId"
			mappings.Status = "Status"
			mappings.Type = "EventType"
			customFieldMappings.Add(New AppointmentCustomFieldMapping("Field1", "Price", FieldValueType.Decimal))
		Finally
			Storage.EndUpdate()
		End Try
	End Sub

	'Initilazing ObjectDataSource
	Protected Sub appointmentsDataSource_ObjectCreated(ByVal sender As Object, ByVal e As ObjectDataSourceEventArgs)
		e.ObjectInstance = New CustomEventDataSource(GetCustomEvents())
	End Sub

	Private Function GetCustomEvents() As CustomEventList
		Dim events As CustomEventList = TryCast(Session("ListBoundModeObjects"), CustomEventList)
		If events Is Nothing Then
			events = New CustomEventList()

			Dim customEvent1 As New CustomEvent()

			customEvent1.Id = customEvent1.GetHashCode()
			customEvent1.StartTime = DateTime.Today.AddHours(1)
			customEvent1.EndTime = DateTime.Today.AddHours(2)
			customEvent1.Subject = "Test event"
			customEvent1.OwnerId = "sbrighton"
			customEvent1.Price = 80

			events.Add(customEvent1)

			Dim customEvent2 As New CustomEvent()

			customEvent2.Id = customEvent2.GetHashCode()
			customEvent2.StartTime = DateTime.Today.AddHours(4)
			customEvent2.EndTime = DateTime.Today.AddHours(5)
			customEvent2.Subject = "Test event 2"
			customEvent2.OwnerId = "rfischer"
			customEvent2.Price = 120

			events.Add(customEvent2)

			Dim customEvent3 As New CustomEvent()

			customEvent3.Id = customEvent3.GetHashCode()
			customEvent3.StartTime = DateTime.Today.AddHours(2)
			customEvent3.EndTime = DateTime.Today.AddHours(3)
			customEvent3.Subject = "Test event 3"
			customEvent3.OwnerId = "amiller"
			customEvent3.Price = 95

			events.Add(customEvent3)

			Session("ListBoundModeObjects") = events
		End If
		Return events
	End Function

	' User generated appointment id    
	Protected Sub ASPxScheduler1_AppointmentInserting(ByVal sender As Object, ByVal e As PersistentObjectCancelEventArgs)
		SetAppointmentId(sender, e)
	End Sub

	Private Sub SetAppointmentId(ByVal sender As Object, ByVal e As PersistentObjectCancelEventArgs)
		Dim storage As ASPxSchedulerStorage = CType(sender, ASPxSchedulerStorage)
		Dim apt As Appointment = CType(e.Object, Appointment)
		storage.SetAppointmentId(apt, apt.GetHashCode())
	End Sub

	Protected Sub ASPxScheduler1_InitAppointmentDisplayText(ByVal sender As Object, ByVal e As AppointmentDisplayTextEventArgs)
		e.Description = String.Format("Price: {0:c}", e.Appointment.CustomFields("Field1"))
	End Sub

	Protected Sub ASPxScheduler1_BeforeExecuteCallbackCommand(ByVal sender As Object, ByVal e As SchedulerCallbackCommandEventArgs)
		Dim scheduler As ASPxScheduler = CType(sender, ASPxScheduler)

		If Convert.ToDecimal(scheduler.SelectedAppointments(0).CustomFields("Field1")) > 100D Then
			If e.CommandId = SchedulerCallbackCommandId.MenuAppointment Then
				e.Command = New CustomMenuAppointmentCallbackCommand(scheduler)
			End If
			If e.CommandId = SchedulerCallbackCommandId.AppointmentsChange Then
				e.Command = New CustomAppointmentsChangeCallbackCommand(scheduler)
			End If
		End If
	End Sub
End Class

Public Class CustomMenuAppointmentCallbackCommand
	Inherits MenuAppointmentCallbackCommand
	Private currentCommandProhibited As Boolean

	Public Sub New(ByVal control As ASPxScheduler)
		MyBase.New(control)

	End Sub

	Protected Overrides Sub ParseParameters(ByVal parameters As String)
		currentCommandProhibited = (parameters = "OpenAppointment" OrElse parameters = "DeleteAppointment" OrElse parameters.Contains("LabelSubMenu") OrElse parameters.Contains("StatusSubMenu"))

		MyBase.ParseParameters(parameters)
	End Sub

	Protected Overrides Sub ExecuteCore()
		If currentCommandProhibited Then
			AddHandler Control.CustomJSProperties, AddressOf ASPxScheduler1_CustomJSProperties
		Else
			MyBase.ExecuteCore()
		End If
	End Sub

	Private Sub ASPxScheduler1_CustomJSProperties(ByVal sender As Object, ByVal e As CustomJSPropertiesEventArgs)
		e.Properties.Add("cpWarning", "The currently selected appointment cannot be modified because of its Price value.")
	End Sub
End Class

Public Class CustomAppointmentsChangeCallbackCommand
	Inherits AppointmentsChangeCommand
	Public Sub New(ByVal control As ASPxScheduler)
		MyBase.New(control)

	End Sub

	Protected Overrides Sub ExecuteCore()
		AddHandler Control.CustomJSProperties, AddressOf ASPxScheduler1_CustomJSProperties
	End Sub

	Private Sub ASPxScheduler1_CustomJSProperties(ByVal sender As Object, ByVal e As CustomJSPropertiesEventArgs)
		e.Properties.Add("cpWarning", "The currently selected appointment cannot be modified because of its Price value.")
	End Sub
End Class