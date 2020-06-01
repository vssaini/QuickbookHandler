'use strict';

const handler = '/Handler.ashx';

var utility = {

    init: () =>
    {
        console.log('Utility initialized.');

        // Set the status based on response
        // We need to do this way because we cannot get response of Intuit due to CORS
        const urlParams = new URLSearchParams(window.location.search);
        const conStatus = urlParams.get('connectionStatus');
        const error = urlParams.get('error');
        const state = urlParams.get('state');

        if (conStatus)
        {
            $.notify('Connection successful!', 'success', { autoHideDelay: 5000 });
            $('#status').text('Connection successful!');

        }
        else if (error)
        {
            $.notify(error, 'error', { autoHideDelay: 10000 });
            $('#status').text(error);
        }
        else if (state)
        {
            $.notify(state, 'error', { autoHideDelay: 10000 });
            $('#status').text(state);
        }
    },

    authenticate: () =>
    {
        $('#authStatus').show();
        const data = { Authenticate: true };
        utility.get(handler, data, function (response)
        {
            window.location = response;
        });
    },

    addCustomer: () =>
    {
        $('#custStatus').show();
        const data = {
            Title: 'Mr',
            GivenName: 'Vicky',
            MiddleName: 'Singh',
            FamilyName: 'Saini',
            PrimaryEmailAddr: 'vssaini@gmail.com',
            PrimaryPhone: '9829478688',
            CompanyName: 'Google'
        };

        utility.postJson(handler, data, function (response)
        {
            if (response.Status)
            {
                $.notify(response.Message, 'success', { autoHideDelay: 5000 });
            } else
            {
                $.notify(response.Message, 'error', { autoHideDelay: 10000 });
            }

            $('#custStatus').hide();

        });
    },

    get: (url, data, successCallback) =>
    {
        $.ajax({
            type: 'GET',
            url: url,
            data: data,
            success: function (response)
            {
                successCallback(response);
            },
            error: function (xhr)
            {
                const errorMessage = `${xhr.status}:${xhr.statusText}`;
                console.error(errorMessage);
                console.log(xhr.responseText);
            }
        });
    },

    postJson: (url, data, successCallback) =>
    {
        const json = JSON.stringify(data);
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            url: url,
            data: json,
            success: function (response)
            {
                successCallback(response);
            },
            error: function (xhr)
            {
                const errorMessage = `${xhr.status}:${xhr.statusText}`;
                console.error(errorMessage);
                console.log(xhr.responseText);
            }
        });
    }
};