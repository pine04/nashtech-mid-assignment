import { useAuth } from "../hooks/useAuth"

const formatRole = (role: string): string => {
    switch (role) {
        case "NormalUser": return "Normal user";
        case "SuperUser": return "Super user";
        default: return ""
    };
}

export const ProfilePage = () => {
    const { user } = useAuth();

    return (
        <div className="p-8">
            <div className="flex flex-col gap-4">
                {
                    user ?
                        <>
                            <h1 className="text-3xl font-bold">My profile</h1>
                            <p><span className="font-bold">First name:</span> {user.firstName}</p>
                            <p><span className="font-bold">Last name:</span> {user.lastName}</p>
                            <p><span className="font-bold">Email:</span> {user.email}</p>
                            <p><span className="font-bold">Role:</span> {formatRole(user.role)}</p>
                        </>
                        :
                        <p>No user data found</p>

                }
            </div>
        </div>
    );
}