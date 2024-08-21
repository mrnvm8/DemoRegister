document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById("customConfirmModal");
    const closeBtn = document.getElementsByClassName("close")[0];
    const confirmDeleteBtn = document.getElementById("confirmDelete");
    const cancelDeleteBtn = document.getElementById("cancelDelete");
    let actionUrl = '';


    // Function to show the modal with custom title and message
    function showModal(title, message, url) {
        document.getElementById("modalTitle").innerText = title;
        document.getElementById("modalMessage").innerText = message;
        actionUrl = url;
        modal.style.display = "block";
    }

    document.querySelectorAll('.delete-button').forEach(button => {
        button.addEventListener('click', function (event) {
            event.preventDefault();
            const url = this.getAttribute('href');
            const title = this.getAttribute('data-title') || "Deleting😠";
            const message = this.getAttribute('data-message') || "Are you sure you want to delete this?";
            showModal(title, message, url);
        });
    });

    confirmDeleteBtn.onclick = function () {
        if (actionUrl) {
            window.location.href = actionUrl;
        }
    };

    cancelDeleteBtn.onclick = function () {
        modal.style.display = "none";
    };

    closeBtn.onclick = function () {
        modal.style.display = "none";
    };

    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    };
});
