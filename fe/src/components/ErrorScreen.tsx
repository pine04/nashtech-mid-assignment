export const ErrorScreen = ({ message = "Something went wrong." }) => {
    return (
        <div className="flex items-center justify-center h-screen bg-gray-100">
            <div className="text-center p-6 bg-white shadow-lg rounded-2xl max-w-md">
                <h1 className="text-2xl font-bold text-red-600 mb-4">Error</h1>
                <p className="text-gray-700">{message}</p>
            </div>
        </div>
    );
}