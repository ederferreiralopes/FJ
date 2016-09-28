
$(document).ready(function () {
    processarVagas();
});

function processarVagas() {
    //table = $('#myTable').DataTable();
    //table.destroy();


    $('#myTable').dataTable({
        "ajax": '../Home/PesquisaEmpresaJson',
        "columns": [
                    { "data": "DataCadastro" },
                    { "data": "Descricao" },
                    { "data": "Cep" },
                    { "data": "Habilidades" },
                    { "data": "Candidato" },
                    { "data": "Distancia" },
                    { "data": "Aderencia" },
                    { "data": "Id" },
        ],
        "language": {
            "emptyTable": "Não existem registros disponiveis",
            "info": "Exibindo _START_ a _END_ de _TOTAL_ registros",
            "infoEmpty": "Nenhum registro encontrado",
            "infoFiltered": "(filtrado de _MAX_ registros)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Exibindo _MENU_ registros",
            "loadingRecords": "Carregando...",
            "processing": "Processando...",
            "search": "Pesquisa:",
            "zeroRecords": "Nenhum resultado encontrado",
            "paginate": {
                "first": "Primeiro",
                "last": "Ultimo",
                "next": "Próximo",
                "previous": "Anterior"
            },
            "aria": {
                "sortAscending": ":Ativar ordenação ascendente",
                "sortDescending": ":Ativar ordenação descendente"
            }
        }
    });
}
