function interactLike(e) {
    fetch("/api/posts/" + e.dataset.postid + "/interaction/like")
        .then((r) => r.json())
        .then((r) => {
            if (r.message == "added") {
                e.classList.remove('text-gray-500', 'hover:text-blue-500');
                e.classList.add('text-blue-500');
                e.querySelector('svg').classList.add('text-blue-500');
                e.querySelector('svg').setAttribute('fill', 'currentColor');
                e.querySelector(".ml-1").innerText = r.count + " Gostos";
            } else {
                e.classList.remove('text-blue-500');
                e.classList.add('text-gray-500', 'hover:text-blue-500');
                e.querySelector('svg').classList.remove('text-blue-500');
                e.querySelector('svg').setAttribute('fill', 'none');
                e.querySelector(".ml-1").innerText = r.count + " Gostos";
            }
        });
}
