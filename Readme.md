# How to allow appointment modifications depending on custom field values


<p>This example illustrates how to customize the <strong>MenuAppointmentCallbackCommand</strong> and <strong>AppointmentsChangeCommand </strong>(see <a href="http://documentation.devexpress.com/#AspNet/CustomDocument5462"><u>Callback Commands</u></a>) to prevent appointments with specific custom field values from modification or deletion. Callback commands are intercepted and substituted by their custom equivalents in the <a href="http://documentation.devexpress.com/#AspNet/DevExpressWebASPxSchedulerASPxScheduler_BeforeExecuteCallbackCommandtopic"><u>ASPxScheduler.BeforeExecuteCallbackCommand Event</u></a> handler. They determine whether or not a current operation should be denied. If an operation is denied, the base callback command logic is skipped and the <a href="http://documentation.devexpress.com/#AspNet/DevExpressWebASPxSchedulerASPxScheduler_CustomJSPropertiestopic"><u>ASPxScheduler.CustomJSProperties Event</u></a> is handled in order to display an error message on the client side (see the client-side <strong>EndCallback</strong> event handler in the Default.aspx file). This approach might be useful because at present there is no appropriate method to customize the popup menu dynamically (see the <a href="https://www.devexpress.com/Support/Center/p/Q346765">Context Menu Items Only for Appointments</a> thread).</p><p><strong>See Also:</strong><br />
<a href="https://www.devexpress.com/Support/Center/p/E3499">End-User Restrictions - How to allow appointment creation or deletion only for specific users</a></p>

<br/>


