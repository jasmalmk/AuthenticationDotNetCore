var createState = function () {
    return "jhckjshdkfuyighsdhgbzjkldhfghksdghklhkjkuydgzdsbvjhbjkldhvjkhjkhjkhskvjjkzsdbjkbkjhSKJHkjhkjhjklHGjklhkghkzjxdgkzjbnkjvbhkdhgkhzdkh";
}

var createNounce = function () {
    return "NounceValuedgvfsbdmngbjkghjkhsdjkhnkj7787";
}

var signIn = function () {
    var redirectUri ="https://localhost:44365/Home/SignIn"
    var responseType = "id_token token";
    var scope = "openid ApiOne";
    var authUrl = "/connect/authorize/callback" +
        "?client_id=client_id_js" +
        "&redirect_uri=" + encodeURIComponent(redirectUri) +
        "&response_type=" + encodeURIComponent(responseType) +
        "&scope=" + encodeURIComponent(scope) +
        "&nonce=" + createNounce() +
        "&state=" + createState();

    var returnURL = encodeURIComponent(authUrl);

    window.location.href = "https://localhost:44309/Auth/Login?ReturnUrl="+returnURL;


    //console.log(url);
}

