import { Link } from "react-router-dom";
import { useCurrentMember } from "./auth/useCurrentMember";

export function MembersLink() {
    const currentMemberQuery = useCurrentMember();

    if (
        !currentMemberQuery.isSuccess 
        || currentMemberQuery.data.role !== "Administrator") 
    {
        return null;
    }

    return <><Link to={`/members`}>Members</Link>{" "}</>
}