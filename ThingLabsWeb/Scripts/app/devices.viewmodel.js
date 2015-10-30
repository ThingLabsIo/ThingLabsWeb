function DevicesViewModel(app, dataModel) {
    var self = this;

    self.myHometown = ko.observable("");

    Sammy(function () {
        this.get('#devices', function () {
            // Make a call to the protected Web API by passing in a Bearer Authorization Header
            $.ajax({
                method: 'get',
                url: app.dataModel.deviceInfoUrl,
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (data) {
                    self.IoTHubConnectionString(data.IoTHubConnectionString);
                }
            });
        });
        this.get('/', function () { this.app.runRoute('get', '#home') });
    });

    return self;
}

app.addViewModel({
    name: "Devices",
    bindingMemberName: "devices",
    factory: DevicesViewModel
});
