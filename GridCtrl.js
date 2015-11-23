app.controller('GridController', function ($scope, $http) {
    $scope.init = { 'count': 10 };
    $scope.getResource = function (params, paramsObj) {
        alert(params);
        var header =
        [
            {
                "key": "Id",
                "name": "Id"
            },
            {
                "key": "FirstName",
                "name": "FirstName"
            },
            {
                "key": "LastName",
                "name": "Last Name"
            },
            {
                "key": "Status",
                "name": "Status"
            },
        {
                "key": "Configure",
        "name": "Configure"
    }
        ];


        var pagination = {

        }
        var urlApi = 'http://localhost/WebApplication4/WebServices/Service1.svc/user/GetUserList';
        return $http.post(urlApi, JSON.stringify({ pageId: (paramsObj.page - 1), pageSize: paramsObj.count })).then(function (response) {

            return {
                'rows': response.data.GetUserListResult,
                'header': header,
                'pagination': 
                     {
                         "count": 10,
                         "page": 1,
                         "pages": 3,
                         "size": 30
                     },
                'sortBy': 'Id',
                'sortOrder': 'asc'
            }
        });
        //alert('abc');


        //return $http.post('http://localhost/WebApplication4/WebServices/Service1.svc/user/GetUserList',  { pageId: 0, pageSize: 10 }).then(function (response) {
        //    return {
        //        'rows': response.count,
        //        'header': header,
        //        'pagination': response.data.pagination,
        //        'sortBy': response.data['sortby'],
        //        'sortOrder': response.data['sortorder']
        //    }
        //});
    }

    $scope.onInit = function () {
        //$http.post("http://localhost/WebApplication4/WebServices/Service1.svc/user/GetUserList", { pageId: '0', pageSize: '10' })
        //.success(function (response) {
        //    alert('success');
        //})
        //.error(function (data, status, header, config) {
        //    alert('error');
        //});
    }


    //$scope.GoToPostFromNotification = function (myNot) {
    //    if (myNot.isPost) {
    //        $http({
    //            url: $scope.baseUrl + 'posts/ChangeNotificationStatus',
    //            method: "POST",
    //            data: { notificationId: myNot.Id, isWeb: true },
    //            withCredentials: true,
    //            headers: {
    //                'Content-Type': 'application/json; charset=utf-8'
    //            }
    //        })
    //        .success(function (response) {
    //            myNot.hasRead = true;
    //        })
    //        .error(function (data, status, header, config) {
    //            // redirect him to login failed page
    //            $scope.errorHandler(data);
    //            alert(data);
    //        });
    //        $location.path("/viewPost/" + myNot.PostId);
    //    }
    //    else {
    //        $scope.showContacts(false, true, false, false);
    //    }
    //}

});