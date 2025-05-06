import { Navigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { AuthLayout } from "../layouts/AuthLayout";

const getDefaultRouteForRole = (role: string): string => {
    switch (role) {
        case "NormalUser": return "/";
        case "SuperUser": return "/admin";
        default: throw new Error("Role unknown.");
    }
}

export const AuthRoute = () => {
    const { user } = useAuth();

    return (
        !user ? <AuthLayout /> : <Navigate to={getDefaultRouteForRole(user.role)} />
    );
}