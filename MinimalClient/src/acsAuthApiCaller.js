export async function GetAcsToken(accessToken) {
    const headers = new Headers();
    const bearer = `Bearer ${accessToken}`;

    headers.append("Authorization", bearer);

    const options = {
        method: "GET",
        headers: headers
    };

    return await fetch("https://localhost:5001/api/token", options)
        .then(response => response.json())
        .catch(error => console.log(error));

}