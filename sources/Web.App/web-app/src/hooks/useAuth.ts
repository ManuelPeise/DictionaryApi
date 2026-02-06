import React from "react";
import AuthContext from "../context/AuthContext";

export const useAuth = () => {  
    const ctx = React.useContext(AuthContext);

    if(ctx === null){
        throw new Error("useAuth must be used within an AuthContextProvider");
    }

    return ctx;
}