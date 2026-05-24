// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $(".add-to-cart-btn").click(function (e) {
        // Since you require login, the backend [Authorize] 
        // will handle this, but you can add UI effects here.
        console.log("Adding to cart...");
    });
});