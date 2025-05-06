import { createContext, ReactNode } from "react";
import { User } from "../types/types";
import { getMyProfile } from "../services/authServices";
import { useQuery, useQueryClient } from "@tanstack/react-query";

interface AuthContextType {
    user: User | undefined;
    resetUser: () => void;
}

export const AuthContext = createContext<AuthContextType | null>(null);

export const AuthContextProvider = ({ children }: { children: ReactNode }) => {
    const queryClient = useQueryClient();

    const { data: user } = useQuery({
        queryKey: ["me"],
        queryFn: getMyProfile,
        retry: false
    });

    const resetUser = () => {
        queryClient.invalidateQueries({ queryKey: ["me"] });
        queryClient.setQueryData(["me"], undefined);
    }

    const value = {
        user,
        resetUser
    }

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    )
}