function getPasswordHash(passwordElement, nonceElement, hashElement)
{
    var password = $('#' + passwordElement).val();
    var nonce = $('#' + nonceElement).val();

    var newHash = $.SHA256(password + nonce)
    $('#' + hashElement).val(newHash);
    $('#' + passwordElement).attr('value', '');
}