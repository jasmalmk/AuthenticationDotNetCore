var config = {
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage }),
    authority: "https://localhost:44309",
    client_id: "client_id_js",
    redirect_uri: "https://localhost:44365/Home/SignIn",
    response_type: "id_token token",
    scope:"openid ApiOne rc.scope ApiTwo"
}

var userManager = new Oidc.UserManager(config);

var signIn = function () {
    userManager.signinRedirect();

   // window.location.href = '/home/index';
}

userManager.getUser().then(user => {
    console.log('user:',user);
    if(user){
        axios.defaults.headers.common["authorization"] = "Bearer " + user.access_token;
    }
})

var callApi = function () {
    axios.get('https://localhost:44379/secret').then(res => {
        console.log(res);
    })
}

axios.interceptors.response.use(
    function (res) { return res; },
    function (err) {
        console.log("axios error:", err.response);

        return Promise.reject(err);
    }
)