// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

$(document).on('focus', '#autocomplete', function () {

    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }

    $(this).autocomplete({
        source: function (request, response) {
            $.getJSON("/Home/AutocompleteSearch", {
                term: extractLast(request.term)
            }, response);
        },
        minLength: 0,
        search: function () {
            // custom minLength
            var term = extractLast(this.value);
            if (term.length < 0) {
                return false;
            }
        },
        focus: function () {
            // prevent value inserted on focus
            return false;
        },
        select: function (event, ui) {
            var terms = split(this.value);
            // remove the current input
            terms.pop();
            // add the selected item
            terms.push(ui.item.value);
            // add placeholder to get the comma-and-space at the end
            terms.push("");
            this.value = terms.join(", ");
            return false;
        }
    }).on('click', function () {
        $(this).autocomplete('search', '');
    })
});

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/serversignalr")
    .build();

hubConnection.on('Toast', function (fullName) {
    $(".toast-body").text("From - " + fullName);

    $(".toast").toast("show");
});

hubConnection.on('Message', function (messageId) {
    $.ajax({
        type: "GET",
        url: "/Home/GetMessage?messageId=" + messageId,
        success: function (result) {
            $("#messages").prepend(result);
            $(document).ready(function () {
                $('[data-bs-toggle="popover"]').popover({ html: true });
            });
        }
    });
});

hubConnection.start();