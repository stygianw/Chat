var chatModel =
    {
        messages: [],
        users: [],
        messages_number: 0
    }

var app = angular.module('chat', []);

app.run(['$http', '$interval', function ($http, $interval) {
    var MessagesRefreshStop = $interval(function () {
        $http.get('/Chat/GetMessages?messno='+ chatModel.messages_number).success(function (data) {
            var newmessages = data;
            for (var i = 0; i < newmessages.messages.length; i++) {
                chatModel.messages.push(newmessages.messages[i]);
            };
            chatModel.messages_number = newmessages.messagesNumber;
        })
    }, 3000);

    var UsersRefreshStop = $interval(function () {
        $http.get('/Chat/GetUsersList').success(function (data) {
            chatModel.users = data.users;
        })
    }, 5000);

}]);

app.controller('ChatController', ['$http', '$scope', function ($http, $scope) {

    $scope.model = chatModel;
    $scope.message = '';
    $scope.send = function () {
        
        $http.post('/Chat/RecordMessage', { message: $scope.message }).success(function () { $scope.message = ''});

    }


}])
