document.addEventListener("DOMContentLoaded", function() {
    fetch("/api/posts").then(r => r.json()).then(data => {
        let posts = data.posts;
        let postsContainer = document.querySelector(".postsContainer");

        if (posts.length === 0) {
            postsContainer.innerHTML = `<span class="text-gray-500 text-center">Não segues pessoas começa a seguir!!</span>`
            return;
        }

        posts.forEach(post => {
            let mockupHtml = `
                <div class="min-w-[600px] mx-auto p-4 bg-white rounded-lg shadow-md" data-postId="${post.PostId}">
                    <div class="flex items-center space-x-4">
                        <img class="w-12 h-12 rounded-full" src="https://picsum.photos/48" alt="User avatar">
                        <div>
                            <h2 class="text-lg font-semibold">${clearText(post.User.Username)}</h2>
                            <span class="text-gray-500 text-sm">${new Date(post.PostedAt).toUTCString()}</span>
                        </div>
                    </div>

                    <div class="mt-4">
                        <p class="text-gray-800">
                            ${clearText(post.Content)}
                        </p>
                        ${post.Files[0] != null ? `<div 
                            class="w-full h-[300px] mt-3 rounded-lg bg-cover bg-no-repeat bg-center overflow-hidden" 
                            style="background-image: url('${post.Files[0]?.FilePath}');">
                        </div>`: ``}
                        
                    </div>

                    <div class="flex justify-between mt-4">
                        <button data-postid=${post.PostId} onclick="interactLike(this)" class="flex items-center text-gray-500 hover:text-blue-500">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-6 h-6">
                                <path stroke-linecap="round" stroke-linejoin="round" d="M6.633 10.25c.806 0 1.533-.446 2.031-1.08a9.041 9.041 0 0 1 2.861-2.4c.723-.384 1.35-.956 1.653-1.715a4.498 4.498 0 0 0 .322-1.672V2.75a.75.75 0 0 1 .75-.75 2.25 2.25 0 0 1 2.25 2.25c0 1.152-.26 2.243-.723 3.218-.266.558.107 1.282.725 1.282m0 0h3.126c1.026 0 1.945.694 2.054 1.715.045.422.068.85.068 1.285a11.95 11.95 0 0 1-2.649 7.521c-.388.482-.987.729-1.605.729H13.48c-.483 0-.964-.078-1.423-.23l-3.114-1.04a4.501 4.501 0 0 0-1.423-.23H5.904m10.598-9.75H14.25M5.904 18.5c.083.205.173.405.27.602.197.4-.078.898-.523.898h-.908c-.889 0-1.713-.518-1.972-1.368a12 12 0 0 1-.521-3.507c0-1.553.295-3.036.831-4.398C3.387 9.953 4.167 9.5 5 9.5h1.053c.472 0 .745.556.5.96a8.958 8.958 0 0 0-1.302 4.665c0 1.194.232 2.333.654 3.375Z" />
                            </svg>
                            <span class="ml-1">Gostar</span>
                        </button>
                        <button onclick="window.location.href='/post/${post.PostId}'" class="flex items-center text-gray-500 hover:text-blue-500">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-6 h-6">
                                <path stroke-linecap="round" stroke-linejoin="round" d="M8.625 12a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0H8.25m4.125 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0H12m4.125 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0h-.375M21 12c0 4.556-4.03 8.25-9 8.25a9.764 9.764 0 0 1-2.555-.337A5.972 5.972 0 0 1 5.41 20.97a5.969 5.969 0 0 1-.474-.065 4.48 4.48 0 0 0 .978-2.025c.09-.457-.133-.901-.467-1.226C3.93 16.178 3 14.189 3 12c0-4.556 4.03-8.25 9-8.25s9 3.694 9 8.25Z" />
                            </svg>
                            <span class="ml-1">Comentar</span>
                        </button>
                        <button class="flex items-center text-gray-500 hover:text-blue-500">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-6 h-6">
                                <path stroke-linecap="round" stroke-linejoin="round" d="M7.217 10.907a2.25 2.25 0 1 0 0 2.186m0-2.186c.18.324.283.696.283 1.093s-.103.77-.283 1.093m0-2.186 9.566-5.314m-9.566 7.5 9.566 5.314m0 0a2.25 2.25 0 1 0 3.935 2.186 2.25 2.25 0 0 0-3.935-2.186Zm0-12.814a2.25 2.25 0 1 0 3.933-2.185 2.25 2.25 0 0 0-3.933 2.185Z" />
                            </svg>
                            <span class="ml-1">Partilhar</span>
                        </button>
                    </div>
                </div>
            `
            postsContainer.innerHTML += mockupHtml;

        });

    })
})