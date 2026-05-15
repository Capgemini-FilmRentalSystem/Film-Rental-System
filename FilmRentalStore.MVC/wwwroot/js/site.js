// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("table[data-client-pagination='true']").forEach(function (table) {
        const pageSize = Number.parseInt(table.dataset.pageSize || "10", 10);
        const tbody = table.querySelector("tbody");
        const pager = table.closest(".table-responsive")?.nextElementSibling;

        if (!tbody || !pager || !pager.classList.contains("client-pagination")) {
            return;
        }

        const rows = Array.from(tbody.querySelectorAll("tr"));
        const totalPages = Math.ceil(rows.length / pageSize);
        let currentPage = 1;

        if (totalPages <= 1) {
            pager.innerHTML = "";
            return;
        }

        const render = function () {
            rows.forEach(function (row, index) {
                const firstItem = (currentPage - 1) * pageSize;
                const lastItem = firstItem + pageSize;
                row.style.display = index >= firstItem && index < lastItem ? "" : "none";
            });

            pager.innerHTML = [
                '<ul class="pagination mb-0">',
                `<li class="page-item ${currentPage === 1 ? "disabled" : ""}"><button class="page-link" type="button" data-page="prev">Previous</button></li>`,
                `<li class="page-item disabled"><span class="page-link">Page ${currentPage} of ${totalPages}</span></li>`,
                `<li class="page-item ${currentPage === totalPages ? "disabled" : ""}"><button class="page-link" type="button" data-page="next">Next</button></li>`,
                "</ul>"
            ].join("");

            pager.querySelectorAll("button[data-page]").forEach(function (button) {
                button.addEventListener("click", function () {
                    if (button.dataset.page === "prev" && currentPage > 1) {
                        currentPage--;
                    }

                    if (button.dataset.page === "next" && currentPage < totalPages) {
                        currentPage++;
                    }

                    render();
                });
            });
        };

        render();
    });
});
