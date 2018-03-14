$(function () {
    //column checkbox select all or cancel
    $("input.select-all").click(function () {
        var checked = this.checked;
        var unchecked = this.checked.false;
        $("input.select-item").each(function (index, item) {
            item.checked = checked;
        });
        $("input.select-item2").each(function (index, item) {
            if (!checked);
            else item.checked = unchecked;
        });
        $("input.select-all2").each(function (index, item) {
            item.checked = unchecked;
        });
    });
});

$(function () {
    //column checkbox select all or cancel
    $("input.select-all2").click(function () {
        var checked = this.checked;
        var unchecked = this.checked.false;
        $("input.select-item2").each(function (index, item) {
            item.checked = checked;
        });
        $("input.select-item").each(function (index, item) {
            if (!checked);
            else item.checked = unchecked;

        });
        $("input.select-all").each(function (index, item) {
            item.checked = unchecked;
        });
    });
});

//COBA INI

function selectOnlyThis2(id) {
    $("input.select-item2").each(function (index, item) {
        if (item.checked) {
            var str_id = item.id;
            $("input.select-item").each(function (index, item2) {
                if (item2.id === str_id)
                    item2.checked = false;
            });
        }
    });
}

function selectOnlyThis(id) {

    $("input.select-item").each(function (index, item) {
        if (item.checked) {
            var str_id = item.id;
            $("input.select-item2").each(function (index, item2) {
                if (item2.id === str_id)
                    item2.checked = false;
            });
        }
    });
}
