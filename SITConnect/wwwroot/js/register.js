function validate() {
    var str = document.getElementById("psw").value;
    if (str.length < 12) {
        document.getElementById("pwd_validate").innerHTML = "Password is very weak and must contain at least 12 characters";
        document.getElementById("pwd_validate").style.color = "Red";
        return ("no_number");
    }
    else if (str.search(/[0-9]/) == -1) {
        document.getElementById("pwd_validate").innerHTML = "Password is very weak and requires at 1 number";
        document.getElementById("pwd_validate").style.color = "Red";
        return ("no_number");
    }
    else if (str.search(/[a-z]/) == -1) {
        document.getElementById("pwd_validate").innerHTML = "Password is weak and requires at 1 lower-case letter";
        document.getElementById("pwd_validate").style.color = "Red";
        return ("no_number");
    }
    else if (str.search(/[A-Z]/) == -1) {
        document.getElementById("pwd_validate").innerHTML = "Password is medium and requires at 1 upper-case letter";
        document.getElementById("pwd_validate").style.color = "#FF4500";
        return ("no_number");
    }
    else if (str.search(/[@$!%*?&]/) == -1) {
        document.getElementById("pwd_validate").innerHTML = "Password is medium and requires at 1 special character";
        document.getElementById("pwd_validate").style.color = "#FF4500";
        return ("no_number");
    }
    else {
        document.getElementById("pwd_validate").innerHTML = "Password is strong";
        document.getElementById("pwd_validate").style.color = "Blue";
    }
}
    function validate1() {
        var str = document.getElementById("psw1").value;
        if (str.length < 12) {
            document.getElementById("pwd_validate1").innerHTML = "Password is very weak and must contain at least 12 characters";
            document.getElementById("pwd_validate1").style.color = "Red";
            return ("less than 12");
        }
        else if (str.search(/[0-9]/) == -1) {
            document.getElementById("pwd_validate1").innerHTML = "Password is very weak and requires at 1 number";
            document.getElementById("pwd_validate1").style.color = "Red";
            return ("no_number");
        }
        else if (str.search(/[a-z]/) == -1) {
            document.getElementById("pwd_validate1").innerHTML = "Password is weak and requires at 1 lower-case letter";
            document.getElementById("pwd_validate").style.color = "Red";
            return ("no_lower");
        }
        else if (str.search(/[A-Z]/) == -1) {
            document.getElementById("pwd_validate1").innerHTML = "Password is medium and requires at 1 upper-case letter";
            document.getElementById("pwd_validate1").style.color = "#FF4500";
            return ("no_upper");
        }
        else if (str.search(/[@$!%*?&]/) == -1) {
            document.getElementById("pwd_validate1").innerHTML = "Password is medium and requires at 1 special character";
            document.getElementById("pwd_validate1").style.color = "#FF4500";
            return ("no_special");
        }
        else if (str != document.getElementById("psw").value) {
            document.getElementById("pwd_validate1").innerHTML = "Both Passwords are not same";
            document.getElementById("pwd_validate1").style.color = "#FF4500";
            return ("no_same");
        }
        else {
            document.getElementById("pwd_validate1").innerHTML = "Password is strong and confirmed";
            document.getElementById("pwd_validate1").style.color = "Blue";
        }

    }