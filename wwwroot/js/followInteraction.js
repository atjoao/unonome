const followBtn = document.getElementById("followBtn");

document.addEventListener("DOMContentLoaded", function() {

    followBtn.addEventListener("click", function() {
        const username = followBtn.dataset.username;

        fetch(`/profile/${username}/follow`, {
            method: "GET"
        }).then(r => {
            return r.json()
        }).then(resp => {

            fetch(`/profile/${username}?json=true`).then(r => r.json()).then(data => {
                document.getElementById("followers").innerText = data.User.Followers.length + " Seguidores";
                document.getElementById("following").innerHTML = data.User.Following.length + " A seguir";
            });

            if (resp.message == "followed") {
                followBtn.innerHTML = "Deixar de seguir";
                followBtn.className = "mt-2 px-4 py-1 text-white bg-red-500 rounded hover:bg-red-600";
                return;
            }

            followBtn.innerHTML = "Seguir";
            followBtn.className = "mt-2 px-4 py-1 text-white bg-blue-500 rounded hover:bg-blue-600"
        
        });
    });
});