import { useQuery } from "@tanstack/react-query";
import { getMyAllowance } from "../services/requestsServices";

export const useAllowance = () => {
    return useQuery({
        queryKey: ["allowance"],
        queryFn: getMyAllowance
    });
}