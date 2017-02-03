function CompletarListbox(accion, updateId, value, sino) {
    $.getJSON("../../Ajax/" + accion + "/" + value,
    function (data) {
        $("#" + updateId).empty(); //VACIA EL CONTENIDO
        if (sino == true) {
            $("#" + updateId).append("<option value=''>TODOS</option>");
        }
        else if (sino == false) {
            $("#" + updateId).append("<option value=''>Seleccion</option>");
        }
        $.each(data, function (i, item) {
            $("#" + updateId).append("<option value='" + item.Id + "'>" + item.Nombre + "</option>");
        });
    });
}