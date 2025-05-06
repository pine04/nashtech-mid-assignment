import { Badge } from "./ui/badge";

interface RequestStatusBadgeProps {
    status: string;
}

export const RequestStatusBadge = ({ status }: RequestStatusBadgeProps) => {
    if (status === "Waiting") {
        return <Badge variant="outline" className="border-blue-800 text-blue-800">Waiting</Badge>;
    }

    if (status === "Approved") {
        return <Badge variant="outline" className="border-green-800 text-green-800">Approved</Badge>;
    }

    if (status === "Rejected") {
        return <Badge variant="outline" className="border-red-800 text-red-800">Rejected</Badge>;
    }

    return <Badge variant="outline" className="border-gray-600 text-gray-600">Unknown</Badge>;
}