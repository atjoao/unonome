let uploadedFile = null;
globalThis["modfied"] = false;

const svgs = {
    add: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" 
                class="w-16 h-16">
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v6m3-3H9m12 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z"></path>
            </svg>`,
    remove: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-6">
                <path stroke-linecap="round" stroke-linejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
            </svg>`
}

function changeCover() {
    const postContainer = document.getElementById('postContainer');

    if (uploadedFile == null) {
        postContainer.style.backgroundImage = "";
        postContainer.style.backgroundSize = "";
        postContainer.style.backgroundRepeat = "";
        postContainer.style.backgroundPosition = "";
        document.getElementById('removeBtn')?.remove();

        postContainer.innerHTML = svgs.add;
        return;
    }
    
    postContainer.style.backgroundImage = `url(${URL.createObjectURL(uploadedFile)})`;
    postContainer.style.backgroundSize = "cover";
    postContainer.style.backgroundRepeat = "no-repeat";
    postContainer.style.backgroundPosition = "center";

    const nodes = postContainer.childNodes;

    for (const node of nodes) {
        if (node.nodeName.toLowerCase() == "svg") {
            node.remove();
        }
    }
}

document.addEventListener("DOMContentLoaded", function() {
    const postContainer = document.getElementById('postContainer');
    const sendPost = document.getElementById('sendPost');
    
    sendPost.addEventListener("click", function() {
        if (!globalThis['modfied']) return;

        const content = document.getElementById('postContent').innerText.trim();

        const formData = new FormData();
        formData.append("content", content);

        if (uploadedFile) {
            formData.append("files", uploadedFile);
        }

        fetch("/api/posts/create", {
            method: "POST",
            body: formData
        }).then(r => {
            if (r.ok) {
                window.location.href = "/";
            } else {
                alert("Ocorreu um erro ao criar o post.");
            }
        });
    });


    postContainer.addEventListener("click", function(e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        
        const input = document.createElement("input");
        input.type = "file";
        input.accept = "image/png; image/jpeg; image/jpg";

        input.addEventListener("change", function () {
            if (input.files) {
                const file = input.files[0];
                if (file) {
                    uploadedFile = file;
                    changeCover();

                    const remove = document.createElement("span");
                    remove.className = "absolute top-0 right-0 bg-gray-500 rounded-lg p-1 z-50 cursor-pointer hover:bg-gray-700 hover:stroke-white";

                    remove.onclick = function() {
                        uploadedFile = null;
                        changeCover();
                    }

                    remove.id = "removeBtn";
                    remove.innerHTML = svgs.remove;

                    document.getElementById("imageContainer").appendChild(remove);
                }
            }
        });

        input.remove();
        input.click();
    });
});
