import { Link } from "react-router-dom";
import { useCurrentMember } from "../auth/useCurrentMember";

type EditMemberLinkProps = {
  memberId: number;
};

export function EditMemberLink({ memberId }: EditMemberLinkProps) {
  const currentMemberQuery = useCurrentMember();

  if (!currentMemberQuery.isSuccess) {
    return null;
  }

  const canEdit =
    currentMemberQuery.data.role === "Administrator" ||
    currentMemberQuery.data.id === memberId;

  if (!canEdit) {
    return null;
  }

  return <Link to={`/members/${memberId}/edit`}>Edit</Link>;
}