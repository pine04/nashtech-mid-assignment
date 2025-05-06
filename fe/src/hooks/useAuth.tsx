import { useMutation, useQueryClient } from "@tanstack/react-query"
import { login, logout, register } from "../services/authServices"
import { clearAccessToken, setAccessToken } from "../services/axios"
import { useNavigate } from "react-router-dom"
import { useCart } from "./useCart"
import { useContext } from "react"
import { AuthContext } from "../contexts/AuthContext"

export const useAuth = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { clearAllBooks } = useCart();
    const authContext = useContext(AuthContext);

    if (authContext == null) {
        throw new Error("useAuth must be used inside AuthContextProvider");
    }

    const registerMutation = useMutation({
        mutationFn: register,
        onSuccess: (data: string) => {
            setAccessToken(data);
            navigate("/books");
            queryClient.invalidateQueries();
            authContext.resetUser();
        }
    });

    const loginMutation = useMutation({
        mutationFn: login,
        onSuccess: (data: string) => {
            setAccessToken(data);
            navigate("/books");
            queryClient.invalidateQueries();
            authContext.resetUser();
        }
    });

    const logoutMutation = useMutation({
        mutationFn: logout,
        onSuccess: () => {
            clearAccessToken();
            clearAllBooks();
            navigate("/login");
            queryClient.invalidateQueries();
            authContext.resetUser();
        },
        onError: () => {
            navigate("/login");
        }
    });

    return {
        user: authContext.user,
        registerMutation,
        loginMutation,
        logoutMutation,
    }
}