import { User } from "../types/types";
import { axiosInstance } from "./axios";

interface RegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    role: number;
}

interface LoginRequest {
    email: string;
    password: string;
}

export const register = async (registerRequest: RegisterRequest): Promise<string> => {
    const response = await axiosInstance.post<string>("/auth/register", registerRequest);
    return response.data;
}

export const login = async (loginRequest: LoginRequest): Promise<string> => {
    const response = await axiosInstance.post<string>("/auth/login", loginRequest);
    return response.data;
};

export const getMyProfile = async (): Promise<User> => {
    const response = await axiosInstance.get<User>("/auth/me");
    return response.data;
}

export const logout = async (): Promise<void> => {
    await axiosInstance.post("/auth/logout");
}