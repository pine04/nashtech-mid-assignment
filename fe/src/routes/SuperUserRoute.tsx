import { Navigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth"
import { Layout } from "../layouts/RootLayout";
import { LoadingSpinner } from "../components/LoadingSpinner";

export const SuperUserRoute = () => {
    const { user } = useAuth();

    if (!user) return <LoadingSpinner />;

    if (user.role === "SuperUser") return <Layout />;

    return <Navigate to="/login" />;
}